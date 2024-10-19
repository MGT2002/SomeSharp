using Habr.Common.DTOs.Requests;
using Habr.Common.DTOs.Views;
using Habr.Common.Exceptions;
using Habr.Common.Resources;
using Habr.Common.Enums;
using Habr.DataAccess.Repositories.Interfaces;
using Habr.DataAccess.Entities;
using Habr.BusinessLogic.Interfaces.PostFeatures;
using Habr.BusinessLogic.Interfaces.UserFeatures;
using AutoMapper;
using Serilog;

namespace Habr.BusinessLogic.Implementations.PostFeatures
{
    public class PostService(
        IPostRepository postRepository,
        IUserManager userManager,
        IMapper mapper)
        : IPostService
    {
        private readonly IPostRepository _postRepository = postRepository;
        private readonly IUserManager _userManager = userManager;
        private readonly IMapper _mapper = mapper;

        public async Task<int> AddAsync(AddPostRequestModel model)
        {
            var post = _mapper.Map<AddPostRequestModel, Post>(model, opt => opt.AfterMap((_, d) =>
            {
                d.CreatedAt = DateTime.UtcNow;
                d.UpdatedAt = d.CreatedAt;
                d.CreatorId = _userManager.UserId;
                d.PublicationDate = d.IsPublished ? DateTime.UtcNow : null;
            }));

            if (post.IsPublished)
            { 
                Log.Information("Post -> {@id} is published", post.Id);
            }

            return await _postRepository.AddAsync(post);
        }

        public async Task DeleteByIdAsync(int id)
        {
            var post = await GetEntityByIdWithCkecksAsync(id);

            if (post.IsPublished)
            {
                throw DomainException.GenerateUnprocessableAction(ExceptionMessageResource.PublishedPostDeletion);
            }

            await _postRepository.DeleteByIdAsync(id);
        }

        public async Task<PostViewModel> GetByIdAsync(int id, GetPostOptions options)
        {
            var post = await GetEntityByIdWithCkecksAsync(id, options);

            return _mapper.Map<Post, PostViewModel>(post);
        }

        public async Task<PublishedPostViewModel> GetPublishedPostAsync(int id)
        {
            var post = await GetEntityByIdWithCkecksAsync(id, GetPostOptions.PublishedOnly
                | GetPostOptions.IncludeCreator | GetPostOptions.IncludeComments, skipAuthorityCheck: true);

            return _mapper.Map<Post, PublishedPostViewModel>(post);
        }

        public async Task<IEnumerable<PostViewModel>> GetByUserIdAsync(int id)
        {
            var posts = await _postRepository.GetByUserIdAsync(id, GetPostOptions.IncludeCreator);

            return _mapper.Map<IEnumerable<Post>, IEnumerable<PostViewModel>>(posts);
        }

        public async Task<List<DraftListItem>> GetDraftListAsync(int userId)
        {
            var posts = await _postRepository.GetByUserIdAsync(userId: userId,
                GetPostOptions.NotPublishedOnly | GetPostOptions.OrderByUpdatedDate);

            return _mapper.Map<IEnumerable<Post>, List<DraftListItem>>(posts);
        }

        public async Task<List<PostListItem>> GetPublishedPostListAsync()
        {
            var posts = await _postRepository.GetAllAsync(GetPostOptions.IncludeCreator
                | GetPostOptions.PublishedOnly | GetPostOptions.OrderByPublicationDate);

            return _mapper.Map<IEnumerable<Post>, List<PostListItem>>(posts);
        }

        public async Task UpdateAsync(UpdatePostRequestModel updated)
        {
            var post = await GetEntityByIdWithCkecksAsync(updated.Id);

            if (post.IsPublished)
            {
                throw DomainException.GenerateUnprocessableAction(ExceptionMessageResource.PublishedPostUpdate);
            }

            ApplyChangesToDraft(updated, draft: post);
            await _postRepository.UpdateAsync(post);
        }

        public async Task MoveToDraftsAsync(int postId)
        {
            var post = await GetEntityByIdWithCkecksAsync(postId, GetPostOptions.IncludeComments);
            
            if (post.Comments.Count > 0)
            {
                throw DomainException.GenerateUnprocessableAction(ExceptionMessageResource.PostWithCommentsMoving);
            }

            post.IsPublished = false;
            post.UpdatedAt = DateTime.UtcNow;
            await _postRepository.UpdateAsync(post);
        }

        public async Task PublishPostAsync(int id)
        {            
            var post = await GetEntityByIdWithCkecksAsync(id, GetPostOptions.IncludeCreator);

            await UpdateAsync(_mapper.Map<Post, UpdatePostRequestModel>(post, opt => opt.AfterMap(
                (_, d) => d.IsPublished = true)));

            Log.Information("Post -> {@id} is published", id);
        }

        private async Task<Post> GetEntityByIdWithCkecksAsync(
            int id,
            GetPostOptions options = GetPostOptions.None,
            bool skipAuthorityCheck = false)
        {
            var post = await _postRepository.GetByIdAsync(id, options) ?? throw DomainException.NotFound;

            if (!skipAuthorityCheck && !HasAuthority(post))
            {
                throw DomainException.AccessDenied;
            }

            return post;
        }

        private void ApplyChangesToDraft(UpdatePostRequestModel model, Post draft)
        {
            _mapper.Map(model, draft, opt => opt.AfterMap((model, draft) =>
            {
                if (model.IsPublished)
                {
                    draft.IsPublished = true;
                    draft.PublicationDate = DateTime.UtcNow;
                }

                draft.UpdatedAt = DateTime.UtcNow;
            }));
        }

        private bool HasAuthority(Post post)
        {
            return post.CreatorId == _userManager.UserId;
        }
    }
}

using Habr.Common.DTOs.Requests;
using Habr.Common.Exceptions;
using Habr.Common.Resources;
using Habr.DataAccess.Entities;
using Habr.DataAccess.Repositories.Interfaces;
using Habr.BusinessLogic.Interfaces.CommentFeatures;
using Habr.BusinessLogic.Interfaces.UserFeatures;
using AutoMapper;

namespace Habr.BusinessLogic.Implementations.CommentFeatures;

public class CommentService(
    IMapper mapper,
    IDeclarationRepository declarationRepository,
    ICommentRepository commentRepository,
    IUserManager userManager)
    : ICommentService
{
    private readonly IMapper _mapper = mapper;
    private readonly IDeclarationRepository _declarationRepository = declarationRepository;
    private readonly ICommentRepository _commentRepository = commentRepository;
    private readonly IUserManager _userManager = userManager;

    public async Task AddCommentAsync(WriteCommentRequestModel model)
    {
        var declaration = await _declarationRepository.GetDeclarationById(model.DeclarationId)
            ?? throw DomainException.GenerateNotFound(ExceptionMessageResource.DeclarationNotFound);

        var isPostReply = declaration is Post;

        if (isPostReply)
        {
            var post = (Post)declaration;

            if (!post.IsPublished)
            {
                throw DomainException.AccessDenied;
            }
        }
        else
        {
            var parentComment = (Comment)declaration;

            if (!parentComment.IsPostReply)
            {
                throw DomainException.GenerateUnprocessableAction(ExceptionMessageResource.CommentNotSupported);
            }
        }

        await _commentRepository.AddAsync(_mapper.Map<WriteCommentRequestModel, Comment>(model, opt => 
            opt.AfterMap((_, d) => {
                d.IsPostReply = isPostReply;
                d.CreatedAt = DateTime.UtcNow;
                d.UpdatedAt = d.CreatedAt;
                d.CreatorId = _userManager.UserId;
            })));
    }

    public async Task DeleteCommentAsync(int id)
    {
        var comment = await _commentRepository.GetByIdAsync(id) ?? throw DomainException.NotFound;

        if (!HasAuthority(comment))
        {
            throw DomainException.AccessDenied;
        }

        await _commentRepository.DeleteByIdAsync(id);
    }

    private bool HasAuthority(Comment comment)
    {
        return comment.Creator!.Id == _userManager.UserId;
    }
}

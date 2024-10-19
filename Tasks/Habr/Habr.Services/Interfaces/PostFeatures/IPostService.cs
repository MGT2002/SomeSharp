using Habr.Common.DTOs.Requests;
using Habr.Common.DTOs.Views;
using Habr.Common.Enums;

namespace Habr.BusinessLogic.Interfaces.PostFeatures;

public interface IPostService
{
    Task<List<PostListItem>> GetPublishedPostListAsync();
    Task<List<DraftListItem>> GetDraftListAsync(int userId);
    Task<IEnumerable<PostViewModel>> GetByUserIdAsync(int id);
    Task<PublishedPostViewModel> GetPublishedPostAsync(int id);
    Task<PostViewModel> GetByIdAsync(int id, GetPostOptions options = GetPostOptions.None);
    Task UpdateAsync(UpdatePostRequestModel updated);
    Task DeleteByIdAsync(int id);
    Task<int> AddAsync(AddPostRequestModel model);
    Task MoveToDraftsAsync(int id);
    Task PublishPostAsync(int id);
}

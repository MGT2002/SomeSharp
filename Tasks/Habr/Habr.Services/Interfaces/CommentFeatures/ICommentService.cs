using Habr.Common.DTOs.Requests;

namespace Habr.BusinessLogic.Interfaces.CommentFeatures;

public interface ICommentService
{
    Task AddCommentAsync(WriteCommentRequestModel model);
    Task DeleteCommentAsync(int id);
}

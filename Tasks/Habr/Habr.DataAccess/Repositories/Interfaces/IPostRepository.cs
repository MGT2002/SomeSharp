using Habr.Common.Enums;
using Habr.DataAccess.Entities;

namespace Habr.DataAccess.Repositories.Interfaces;

public interface IPostRepository : IRepository<Post, int>
{
    Task<IEnumerable<Post>> GetAllAsync(GetPostOptions options);
    Task<IEnumerable<Post>> GetByUserIdAsync(int userId, GetPostOptions options);
    Task<Post?> GetByIdAsync(int id, GetPostOptions options);
}

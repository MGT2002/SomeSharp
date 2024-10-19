using Habr.DataAccess.Entities;

namespace Habr.DataAccess.Repositories.Interfaces;

public interface IUserRepository : IRepository<User, int>
{
    Task<User?> GetUserByEmailAsync(string email, bool asNoTracking = true);
}

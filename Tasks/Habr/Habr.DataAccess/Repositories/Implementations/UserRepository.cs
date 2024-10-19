using Habr.DataAccess.Entities;
using Habr.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Habr.DataAccess.Repositories.Implementations;

public class UserRepository(DataContext context) : Repository<User, int>(context),
    IUserRepository
{
    public async Task<User?> GetUserByEmailAsync(string email, bool asNoTracking = true)
    {
        if (asNoTracking)
        {
            return await context.Users.AsNoTracking()
                .SingleOrDefaultAsync(u => u.Email == email);
        }
        else
        {
            return await context.Users.SingleOrDefaultAsync(u => u.Email == email);
        }
    }
}

using Habr.DataAccess.Entities;
using Habr.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Habr.DataAccess.Repositories.Implementations;

public class CommentRepository(DataContext context) :
    Repository<Comment, int>(context), ICommentRepository
{
    private readonly DataContext _context = context;

    public override async Task<Comment?> GetByIdAsync(int id, bool asNoTracking = true)
    {
        var query = _context.Comments.AsQueryable();

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.Include(c => c.Creator).SingleOrDefaultAsync(c => c.Id == id);
    }
}

using Habr.Common.Enums;
using Habr.DataAccess.Entities;
using Habr.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Habr.DataAccess.Repositories.Implementations;

public class PostRepository(DataContext context)
    : Repository<Post, int>(context), IPostRepository
{
    private readonly DataContext context = context;

    public async Task<IEnumerable<Post>> GetAllAsync(GetPostOptions options)
    {
        var query = context.Posts.AsQueryable();

        query = ApplyPublicationOptions(options, query);
        query = ApplyOrderingOptions(options, query);
        query = ApplyCommonOptions(options, query);

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Post>> GetByUserIdAsync(int userId, GetPostOptions options)
    {
        var query = context.Posts.Where(p => p.CreatorId == userId);
        query = ApplyPublicationOptions(options, query);
        query = ApplyOrderingOptions(options, query);
        query = ApplyCommonOptions(options, query);

        return await query.ToListAsync();
    }

    public async Task<Post?> GetByIdAsync(int id, GetPostOptions options)
    {
        var query = context.Posts.AsQueryable();
        query = ApplyPublicationOptions(options, query);
        query = ApplyCommonOptions(options, query);

        return await query.SingleOrDefaultAsync(p => p.Id == id);
    }

    private static IQueryable<Post> ApplyOrderingOptions(GetPostOptions options, IQueryable<Post> query)
    {
        if (options.HasFlag(GetPostOptions.OrderByPublicationDate))
        {
            query = query.OrderBy(p => p.PublicationDate);
        }
        else if (options.HasFlag(GetPostOptions.OrderByUpdatedDate))
        {
            query = query.OrderBy(p => p.UpdatedAt);
        }

        return query;
    }

    private static IQueryable<Post> ApplyPublicationOptions(GetPostOptions options, IQueryable<Post> query)
    {
        if (options.HasFlag(GetPostOptions.PublishedOnly))
        {
            query = query.Where(p => p.IsPublished);
        }
        else if (options.HasFlag(GetPostOptions.NotPublishedOnly))
        {
            query = query.Where(p => !p.IsPublished);
        }

        return query;
    }

    private static IQueryable<Post> ApplyCommonOptions(GetPostOptions options, IQueryable<Post> query)
    {
        if (!options.HasFlag(GetPostOptions.AsTracking))
        {
            query = query.AsNoTracking();
        }
        if (options.HasFlag(GetPostOptions.IncludeCreator))
        {
            query = query.Include(p => p.Creator);
        }
        if (options.HasFlag(GetPostOptions.IncludeComments))
        {
            query = query.Include(p => p.Comments)
                .ThenInclude(c => c.Comments).ThenInclude(c => c.Creator);
            query = query.Include(p => p.Comments).ThenInclude(c => c.Creator);
        }

        return query;
    }
}

using Habr.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Habr.DataAccess.Entities.Base;

namespace Habr.DataAccess.Repositories.Implementations;


public abstract class Repository<TEntity, TId>(DbContext context)
    : IRepository<TEntity, TId> where TEntity : Entity<TId>
{
    private readonly DbContext context = context;
    private readonly DbSet<TEntity> dbSet = context.Set<TEntity>();

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(
        bool asNoTracking = true)
    {
        if (asNoTracking)
        {
            return await dbSet.AsNoTracking().ToListAsync();
        }
        return await dbSet.ToListAsync();
    }
    public virtual async Task<IEnumerable<TEntity>> GetAsync(
        int skip, int take, bool asNoTracking = true)
    {
        if (asNoTracking)
        {
            return await dbSet.AsNoTracking().Skip(skip).Take(take).ToListAsync();
        }
        return await dbSet.Skip(skip).Take(take).ToListAsync();
    }

    public virtual async Task<TEntity?> GetByIdAsync(
        TId id, bool asNoTracking = true)
    {
        if (asNoTracking)
        {
            return await dbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id!.Equals(id));
        }
        return await dbSet.FirstOrDefaultAsync(e => e.Id!.Equals(id));
    }

    public virtual async Task<TId> AddAsync(TEntity entity)
    {
        dbSet.Add(entity);
        await context.SaveChangesAsync();
        return entity.Id;
    }

    public virtual async Task UpdateAsync(TEntity entity)
    {
        dbSet.Update(entity);
        await context.SaveChangesAsync();
        dbSet.Entry(entity).State = EntityState.Detached;
    }

    public virtual async Task<bool> DeleteByIdAsync(TId id)
    {
        var entity = await GetByIdAsync(id, false);
        if (entity != null)
        {
            dbSet.Remove(entity);
            await context.SaveChangesAsync();
            return true;
        }

        return false;
    }
}




using Habr.DataAccess.Entities.Base;

namespace Habr.DataAccess.Repositories.Interfaces;

public interface IRepository<TEntity, TId> where TEntity : Entity<TId>
{
    Task<IEnumerable<TEntity>> GetAllAsync(bool asNoTracking = true);
    Task<IEnumerable<TEntity>> GetAsync(int skip, int take, bool asNoTracking = true);
    Task<TEntity?> GetByIdAsync(TId id, bool asNoTracking = true);
    Task<TId> AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task<bool> DeleteByIdAsync(TId id);
}

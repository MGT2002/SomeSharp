#nullable disable
namespace Habr.DataAccess.Entities.Base;

public abstract class Entity<T>
{
    public T Id { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

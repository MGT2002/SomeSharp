using Habr.DataAccess.Entities.Base;

namespace Habr.DataAccess.Entities;

public abstract class Declaration : Entity<int>
{
    public string Text { get; set; } = null!;
    public int? CreatorId { get; set; }
    public User? Creator { get; set; }
    public ICollection<Comment> Comments { get; set; } =
        new HashSet<Comment>();
}

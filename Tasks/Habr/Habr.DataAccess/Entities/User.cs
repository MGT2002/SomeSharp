#nullable disable
using Habr.DataAccess.Entities.Base;

namespace Habr.DataAccess.Entities
{
    public class User : Entity<int>
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public ICollection<Post> Posts { get; set; } =
#pragma warning disable IDE0028 // Simplify collection initialization
            new HashSet<Post>();
        public ICollection<Comment> Comments { get; set; } =
            new HashSet<Comment>();
#pragma warning restore IDE0028 // Simplify collection initialization
    }
}

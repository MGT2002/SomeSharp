#nullable disable
namespace Habr.DataAccess.Entities
{
    public class Post : Declaration
    {
        public string Title { get; set; }

        public bool IsPublished { get; set; }

        public DateTime? PublicationDate { get; set; }
    }
}

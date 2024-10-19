#nullable disable

namespace Habr.Common.DTOs.Views;

public class PostListItem
{
    public int Id { get; set; }
    public string Title { get; set; }

    public string AuthorEmail { get; set; }

    public DateTime? PublicationDate { get; set; }
}

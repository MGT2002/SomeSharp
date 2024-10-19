namespace Habr.Common.DTOs.Views;

public class PublishedPostViewModel
{
    public string Title { get; set; } = null!;

    public string Text { get; set; } = null!;

    public string? AuthorsEmail { get; set; }

    public DateTime PublishDate { get; set; }

    public ICollection<CommentViewModel> Comments { get; set; } = [];
}

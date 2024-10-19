namespace Habr.Common.DTOs.Views;

public class PostViewModel
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Text { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int? CreatorId { get; set; }

    public string? CreatorName { get; set; }

    public string? CreatorEmail { get; set; }

    public bool IsPublished { get; set; }

    public DateTime? PublicationDate { get; set; }
}

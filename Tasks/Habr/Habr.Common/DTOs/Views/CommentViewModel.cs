namespace Habr.Common.DTOs.Views;

public class CommentViewModel
{
    public int Id { get; set; }
    public string Text { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int? CreatorId { get; set; }
    public string? CreatorName { get; set; }
    public string? CreatorEmail { get; set; }
    public int ParrentId { get; set; }
    public bool IsPostReply { get; set; }
}

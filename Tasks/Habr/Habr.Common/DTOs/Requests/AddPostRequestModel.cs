namespace Habr.Common.DTOs.Requests;

public class AddPostRequestModel
{
    public string Title { get; set; } = string.Empty;

    public string Text { get; set; } = string.Empty;

    public bool IsPublished { get; set; }
}

using System.Text.Json.Serialization;

namespace Habr.Common.DTOs.Requests;

public class WriteCommentRequestModel
{
    public int DeclarationId { get; set; }

    public string Text { get; set; } = string.Empty;
}

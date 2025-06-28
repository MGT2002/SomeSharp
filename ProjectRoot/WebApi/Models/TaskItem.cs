namespace WebApi.Models;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public int OriginalEstimate { get; set; }
    public int BusinessValue { get; set; }

    public override string ToString()
    {
        return $"TaskItem(Id: {Id}, Title: {Title}, Description: {Description}, IsCompleted: {IsCompleted}, CreatedAt: {CreatedAt}, OriginalEstimate: {OriginalEstimate}, BusinessValue: {BusinessValue})";
    }
}

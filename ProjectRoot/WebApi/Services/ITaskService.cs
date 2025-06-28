using WebApi.Models;

namespace WebApi.Services;

public interface ITaskService
{
    Task<IEnumerable<TaskItem>> GetTasksAsync();
    Task<TaskItem?> GetTaskAsync(int id);
    Task<TaskItem> CreateTaskAsync(TaskItem task);
    Task<bool> UpdateTaskAsync(int id, TaskItem task);
    Task<bool> DeleteTaskAsync(int id);
}

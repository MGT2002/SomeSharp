using System.Collections.Generic;
using System.Threading.Tasks;
using GenericProject.WebApi.Models;

namespace GenericProject.WebApi.Services
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskItem>> GetTasksAsync();
        Task<TaskItem?> GetTaskAsync(int id);
        Task<TaskItem> CreateTaskAsync(TaskItem task);
        Task<bool> UpdateTaskAsync(int id, TaskItem task);
        Task<bool> DeleteTaskAsync(int id);
    }
}

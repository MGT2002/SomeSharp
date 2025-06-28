using WebApi.Data;
using WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Services;

public class TaskService(AppDbContext context) : ITaskService
{
    private readonly AppDbContext context = context;

    public async Task<IEnumerable<TaskItem>> GetTasksAsync()
    {
        return await context.Tasks.ToListAsync();
    }

    public async Task<TaskItem?> GetTaskAsync(int id)
    {
        return await context.Tasks.FindAsync(id);
    }

    public async Task<TaskItem> CreateTaskAsync(TaskItem task)
    {
        task.CreatedAt = DateTime.UtcNow;
        context.Tasks.Add(task);
        await context.SaveChangesAsync();
        return task;
    }

    public async Task<bool> UpdateTaskAsync(int id, TaskItem task)
    {
        if (id != task.Id)
            return false;
        context.Entry(task).State = EntityState.Modified;
        try
        {
            await context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!context.Tasks.Any(e => e.Id == id))
                return false;
            throw;
        }
    }

    public async Task<bool> DeleteTaskAsync(int id)
    {
        var task = await context.Tasks.FindAsync(id);
        if (task == null)
            return false;
        context.Tasks.Remove(task);
        await context.SaveChangesAsync();
        return true;
    }
}

using System.Threading.Tasks;
using WebApi.Data;
using WebApi.Models;
using WebApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Common;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace WebApi.Tests.Integration;

[TestClass]
public class TaskServiceTests
{
    private IServiceScope scope = null!;
    private TaskService service = null!;
    private DbTransaction transaction = null!;

    [TestInitialize]
    public void TestInit()
    {
        var factory = new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<Program>();
        scope = factory.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var conn = dbContext.Database.GetDbConnection();
        if (conn.State != System.Data.ConnectionState.Open)
            conn.Open();

        transaction = conn.BeginTransaction();
        dbContext.Database.UseTransaction(transaction);
        service = new TaskService(dbContext);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        transaction.Rollback();
        transaction.Dispose();

        scope.Dispose();
    }

    [TestMethod]
    public async Task CreateAndGetTask_WorksCorrectly()
    {
        var newTask = new TaskItem { Title = "Integration Test", Description = "Test Desc" };
        var created = await service.CreateTaskAsync(newTask);
        Assert.IsNotNull(created);
        Assert.AreEqual("Integration Test", created.Title);

        var fetched = await service.GetTaskAsync(created.Id);
        Assert.IsNotNull(fetched);
        Assert.AreEqual(created.Id, fetched.Id);

        Console.WriteLine("Task created and fetched successfully:");
        Console.WriteLine(fetched);
    }

    [TestMethod]
    public async Task GetTasksAsync_ReturnsAllTasks()
    {
        // Arrange: Add two tasks
        var task1 = await service.CreateTaskAsync(new TaskItem { Title = "Task 1", Description = "Desc 1" });
        var task2 = await service.CreateTaskAsync(new TaskItem { Title = "Task 2", Description = "Desc 2" });

        // Act
        var tasks = (await service.GetTasksAsync()).ToList();

        // Assert
        Assert.IsTrue(tasks.Any(t => t.Id == task1.Id));
        Assert.IsTrue(tasks.Any(t => t.Id == task2.Id));
    }

    [TestMethod]
    public async Task UpdateTaskAsync_UpdatesTask()
    {
        // Arrange: Create a task
        var task = await service.CreateTaskAsync(new TaskItem { Title = "ToUpdate", Description = "Before" });
        task.Title = "Updated Title";
        task.Description = "After";
        task.IsCompleted = true;

        // Act
        var result = await service.UpdateTaskAsync(task.Id, task);
        var updated = await service.GetTaskAsync(task.Id);

        // Assert
        Assert.IsTrue(result);
        Assert.IsNotNull(updated);
        Assert.AreEqual("Updated Title", updated!.Title);
        Assert.AreEqual("After", updated.Description);
        Assert.IsTrue(updated.IsCompleted);
    }

    [TestMethod]
    public async Task UpdateTaskAsync_ReturnsFalseForMismatchedId()
    {
        var task = await service.CreateTaskAsync(new TaskItem { Title = "Mismatch", Description = "Test" });
        var result = await service.UpdateTaskAsync(task.Id + 1, task);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task DeleteTaskAsync_DeletesTask()
    {
        // Arrange: Create a task
        var task = await service.CreateTaskAsync(new TaskItem { Title = "ToDelete", Description = "Delete me" });

        // Act
        var result = await service.DeleteTaskAsync(task.Id);
        var deleted = await service.GetTaskAsync(task.Id);

        // Assert
        Assert.IsTrue(result);
        Assert.IsNull(deleted);
    }

    [TestMethod]
    public async Task DeleteTaskAsync_ReturnsFalseForNonexistentId()
    {
        var result = await service.DeleteTaskAsync(-12345); // unlikely to exist
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task CreateTask_WithOriginalEstimateAndBusinessValue_WorksCorrectly()
    {
        var newTask = new TaskItem {
            Title = "EstimateTest",
            Description = "Test with estimates",
            OriginalEstimate = 8,
            BusinessValue = 13
        };
        var created = await service.CreateTaskAsync(newTask);
        Assert.IsNotNull(created);
        Assert.AreEqual(8, created.OriginalEstimate);
        Assert.AreEqual(13, created.BusinessValue);

        var fetched = await service.GetTaskAsync(created.Id);
        Assert.IsNotNull(fetched);
        Assert.AreEqual(8, fetched!.OriginalEstimate);
        Assert.AreEqual(13, fetched.BusinessValue);
    }

    [TestMethod]
    public async Task UpdateTask_OriginalEstimateAndBusinessValue_WorksCorrectly()
    {
        var task = await service.CreateTaskAsync(new TaskItem {
            Title = "UpdateEstimate",
            Description = "Before update",
            OriginalEstimate = 3,
            BusinessValue = 5
        });
        task.OriginalEstimate = 21;
        task.BusinessValue = 34;
        var result = await service.UpdateTaskAsync(task.Id, task);
        var updated = await service.GetTaskAsync(task.Id);
        Assert.IsTrue(result);
        Assert.IsNotNull(updated);
        Assert.AreEqual(21, updated!.OriginalEstimate);
        Assert.AreEqual(34, updated.BusinessValue);
    }
}

using System.Net.Http.Json;
using WebUi.Models;

namespace WebUi.Services
{
    public class TaskService
    {
        private readonly HttpClient _http;
        public TaskService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<TaskItem>?> GetTasksAsync()
            => await _http.GetFromJsonAsync<List<TaskItem>>("api/tasks");

        public async Task<TaskItem?> GetTaskAsync(int id)
            => await _http.GetFromJsonAsync<TaskItem>($"api/tasks/{id}");

        public async Task<TaskItem?> CreateTaskAsync(TaskItem task)
        {
            var response = await _http.PostAsJsonAsync("api/tasks", task);
            return await response.Content.ReadFromJsonAsync<TaskItem>();
        }

        public async Task<bool> UpdateTaskAsync(int id, TaskItem task)
        {
            var response = await _http.PutAsJsonAsync($"api/tasks/{id}", task);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/tasks/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}

using System.Text.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Core.Entities;
using Core.Repositories;

namespace JsonInfrastructure;

public class CreatureRepository(
    HttpClient httpClient,
    IWebAssemblyHostEnvironment environment)
    : ICreatureRepository
{
    private readonly HttpClient httpClient = httpClient;
    private readonly IWebAssemblyHostEnvironment environment = environment;

    public async Task<Creature> GetCreatureAsync(string type)
    {
        var jsonFilePath = new Uri($"{environment.BaseAddress}/Creatures/creatures.json");

        try
        {
            var jsonData = await httpClient.GetStringAsync(jsonFilePath);
            var creatures = JsonSerializer.Deserialize<List<Creature>>(jsonData) ??
                throw new NullReferenceException("Cannot retrieve creatures from the JSON file!");

            var creature = creatures.Find(c => c.Name.Equals(type, StringComparison.OrdinalIgnoreCase));

            return creature ?? throw new KeyNotFoundException($"Creature with name '{type}' not found!");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error fetching creature {type}: {ex.Message}");
        }
    }
}

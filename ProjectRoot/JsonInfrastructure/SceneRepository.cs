using Core.Entities;
using Core.Repositories;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Text.Json;

namespace JsonInfrastructure;

public class SceneRepository(
    HttpClient httpClient,
    IWebAssemblyHostEnvironment environment
    ) : ISceneRepository
{
    private readonly HttpClient httpClient = httpClient;
    private readonly IWebAssemblyHostEnvironment environment = environment;

    public async Task<IEnumerable<GameState>> GetScene(string sceneName)
    {
        var jsonFilePath = new Uri($"{environment.BaseAddress}/Scenes/{sceneName}.json");
        var jsonData = await httpClient.GetStringAsync(jsonFilePath);
        var gameStates = JsonSerializer.Deserialize<IEnumerable<GameState>>(jsonData) ??
            throw new NullReferenceException("Cannot get scenes from json!");

        return gameStates;
    }
}

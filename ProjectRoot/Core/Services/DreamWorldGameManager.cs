using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using Core.Scenes;

namespace Core.Services;

public class DreamWorldGameManager(IServiceProvider serviceProvider) : IGameManager
{
    private readonly IServiceProvider serviceProvider = serviceProvider;
    private Scene currentScene = null!;
    private PlayerState playerState = new("Player", 1, 100);
    private PlayerInventory playerInventory = new([new() { Name = "Hope", Description = "Unique Item" }]);

    public GameStateDTO GetCurrentState() => new(currentScene.GetCurrentState(), playerState);
    public GameStateDTO GetNextState(int playerAction) => new(currentScene.GetNextState(playerAction), playerState);

    public async Task StartNewGame()
    {
        currentScene = new GameStartScene(serviceProvider);
        await currentScene.Initialize(playerState, playerInventory);
    }

    public void LoadGame(uint slot)
    {
        throw new NotImplementedException();
    }

    public void SaveGame(uint slot)
    {
        throw new NotImplementedException();
    }

    public PlayerInventory GetPlayerInventory()
    {
        throw new NotImplementedException();
    }
}

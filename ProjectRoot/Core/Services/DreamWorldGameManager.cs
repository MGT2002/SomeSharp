using Core.Entities;
using Core.Interfaces;
using Core.Scenes;

namespace Core.Services;

public class DreamWorldGameManager(IServiceProvider serviceProvider) : IGameManager
{
    private readonly IServiceProvider serviceProvider = serviceProvider;
    private Scene currentScene = null!;

    public GameState GetCurrentState() => currentScene.GetCurrentState();
    public GameState GetNextState(PlayerAction playerAction) => currentScene.GetNextState(playerAction);

    public void StartNewGame()
    {
        currentScene = new GameStartScene(serviceProvider);
        currentScene.Initialize();
    }

    public void LoadGame(uint slot)
    {
        throw new NotImplementedException();
    }

    public void SaveGame(uint slot)
    {
        throw new NotImplementedException();
    }
}

using Core.Constants;
using Core.Entities;
using Core.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Scenes;

internal class GameStartScene(IServiceProvider serviceProvider) : Scene(serviceProvider)
{
    long currentGameStateId = 0;
    private Dictionary<long, GameState> gameStartScene = null!;
    private ISceneRepository sceneRepository = null!;
    PlayerState playerState = null!;
    PlayerInventory playerInventory = null!;

    public override string Name => SceneNames.Start;

    public override async Task Initialize(PlayerState playerState, PlayerInventory playerInventory)
    {
        sceneRepository = serviceProvider.GetRequiredService<ISceneRepository>();
        gameStartScene =  (await sceneRepository.GetScene(Name)).ToDictionary(gs => gs.Id);
        currentGameStateId = gameStartScene.Keys.First();
        this.playerState = playerState;
        this.playerInventory = playerInventory;
    }

    public override GameState GetCurrentState()
    {
        return gameStartScene[currentGameStateId];
    }

    public override GameState GetNextState(int playerAction)
    {
        var state = gameStartScene[currentGameStateId];
        if (state.Type.Equals(GameStateType.Fight))
            return Fight(playerAction);
        if(state.Type.Equals(GameStateType.Dialog))
            return Dialog(playerAction);

        throw new NotImplementedException($"There are no implementation for state type-{state.Type}.");
    }

    private GameState Dialog(int playerAction)
    {
        currentGameStateId = gameStartScene[currentGameStateId].PlayerAction.CorrespondingGameStateIds[playerAction];
        return gameStartScene[currentGameStateId];
    }

    private GameState Fight(int playerAction)
    {
        throw new NotImplementedException();
    }
}

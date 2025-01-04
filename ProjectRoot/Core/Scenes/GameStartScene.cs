using Core.Constants;
using Core.Entities;
using Core.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Scenes;

internal class GameStartScene(IServiceProvider serviceProvider) : Scene(serviceProvider)
{
    private IEnumerator<GameState> gameStartScene = null!;
    private ISceneRepository sceneRepository = null!;

    public override string Name => SceneNames.Start;
    
    public override void Initialize()
    {
        sceneRepository = serviceProvider.GetRequiredService<ISceneRepository>();
        gameStartScene = sceneRepository.GetScene(Name).GetEnumerator();
    }

    public override GameState GetCurrentState()
    {
        return gameStartScene.Current;
    }

    public override GameState GetNextState(PlayerAction playerAction)
    {
        throw new NotImplementedException();
    }

}

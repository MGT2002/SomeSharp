using Core.Entities;

namespace Core.Scenes;

internal abstract class Scene(IServiceProvider serviceProvider)
{
    protected readonly IServiceProvider serviceProvider = serviceProvider;

    public abstract string Name { get; }

    public abstract GameState GetCurrentState();
    public abstract GameState GetNextState(PlayerAction playerAction);
    public abstract void Initialize();
}

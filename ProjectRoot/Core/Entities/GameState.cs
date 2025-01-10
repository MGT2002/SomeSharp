using Core.Constants;

namespace Core.Entities;

public record class GameState(
    long Id,
    string CurrentContext,
    GameStateType Type,
    PlayerAction PlayerAction
    )
{
    public GameState() : this(0, string.Empty,
        GameStateType.Dialog, default!){ }
}

using Core.Constants;

namespace Core.Entities;

public record GameState(
    long Id,
    string CurrentContext,
    GameStateType Type,
    PlayerAction PlayerAction
    );

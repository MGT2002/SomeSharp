using Core.Constants;
using Core.Entities;

namespace Core.DTOs;

public record class GameStateDTO
{
    public string CurrentContext { get; init; }
    public GameStateType Type { get; init; }
    public string[] PlayerActionOptions { get; init; }
    public PlayerState PlayerState { get; init; }

    public GameStateDTO(string currentContext, GameStateType type, string[] playerActionOptions, PlayerState playerState)
    {
        CurrentContext = currentContext;
        Type = type;
        PlayerActionOptions = playerActionOptions;
        PlayerState = playerState;
    }

    public GameStateDTO(GameState gameState, PlayerState playerState)
    {
        CurrentContext = gameState.CurrentContext;
        Type = gameState.Type;
        PlayerActionOptions = gameState.PlayerAction.Options;
        PlayerState = playerState;
    }
}

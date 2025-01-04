using Core.Entities;

namespace Core.Interfaces;

public interface IGameManager
{
    GameState GetCurrentState();
    GameState GetNextState(PlayerAction playerAction);
    void SaveGame(uint slot);
    void LoadGame(uint slot);
    void StartNewGame();
}

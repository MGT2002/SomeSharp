using Core.DTOs;
using Core.Entities;

namespace Core.Interfaces;

public interface IGameManager
{
    GameStateDTO GetCurrentState();
    GameStateDTO GetNextState(int playerAction);
    PlayerInventory GetPlayerInventory();
    void SaveGame(uint slot);
    void LoadGame(uint slot);
    Task StartNewGame();
}

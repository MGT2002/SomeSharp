using Core.Entities;

namespace Core.Repositories;

public interface ISceneRepository
{
    IEnumerable<GameState> GetScene(string sceneName);
}

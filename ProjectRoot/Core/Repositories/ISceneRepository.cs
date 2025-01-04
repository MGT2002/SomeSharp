using Core.Entities;

namespace Core.Repositories;

public interface ISceneRepository
{
    Task<IEnumerable<GameState>> GetScene(string sceneName);
}

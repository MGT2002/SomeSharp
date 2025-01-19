using Core.Entities;

namespace Core.Repositories;

public interface ICreatureRepository
{
    Task<Creature> GetCreatureAsync(string type);
}

using Core.Entities;
using Core.Repositories;

namespace Core.Services.Factories;

internal class CreatureFactory(ICreatureRepository creatureRepository)
{
    private readonly ICreatureRepository creatureRepository = creatureRepository;
    private readonly Dictionary<string, Creature> PrototypeCollection = [];

    public async Task<Creature> GetCreature(string type)
    {
        PrototypeCollection.TryGetValue(type, out Creature? prototype);
        if (prototype is null)
        { 
            PrototypeCollection.Add(type, prototype = await creatureRepository.GetCreatureAsync(type));
        }       

        return prototype.Clone();
    }
}

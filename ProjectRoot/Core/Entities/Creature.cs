namespace Core.Entities;

public class Creature(string name, decimal health, int experience)
{
    public string Name { get; set; } = name;
    public decimal Health { get; set; } = health;
    public int Experience { get; } = experience;

    public Creature Clone()
    {
        return (Creature)MemberwiseClone();
    }
}

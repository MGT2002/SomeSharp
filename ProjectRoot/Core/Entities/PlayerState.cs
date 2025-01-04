namespace Core.Entities;

public class PlayerState(string name, uint level, decimal health)
{
    public string Name { get; set; } = name;
    public uint Level { get; set; } = level;
    public decimal Health { get; set; } = health;
}

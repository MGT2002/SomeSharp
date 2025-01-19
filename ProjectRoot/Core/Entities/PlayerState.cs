namespace Core.Entities;

public class PlayerState(string name, uint level, ulong experience, decimal health)
{
    public string Name { get; set; } = name;
    public uint Level { get; set; } = level;
    public ulong Experience { get; } = experience;
    public decimal Health { get; set; } = health;
}

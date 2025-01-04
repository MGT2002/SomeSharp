namespace Core.Entities;

public class PlayerInventory(List<Item> Items)
{
    public List<Item> Items { get; } = Items;
}

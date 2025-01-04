namespace Core.Entities;

public class Item : GameObject
{
    public decimal? Price { get; set; }
    public int Quantity { get; set; }
    public bool IsQuestItem { get; set; }
}

using Microsoft.EntityFrameworkCore;

namespace Northwind.Services.EntityFramework.Entities;

[PrimaryKey(nameof(OrderID), nameof(ProductID))]
public class OrderDetail
{
    public long OrderID { get; set; }

    public Order? Order { get; set; }

    public long ProductID { get; set; }

    public Product? Product { get; set; }

    public double UnitPrice { get; set; }

    public long Quantity { get; set; }

    public double Discount { get; set; }
}

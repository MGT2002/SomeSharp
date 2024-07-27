using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Northwind.Services.EntityFramework.Entities;

public class Product
{
    [Key]
    public long ProductID { get; set; }

    public string? ProductName { get; set; }

    public long SupplierID { get; set; }

    [ForeignKey(nameof(SupplierID))]
    public Supplier? Supplier { get; set; }

    public long CategoryID { get; set; }

    [ForeignKey(nameof(CategoryID))]
    public Category? Category { get; set; }

    public string? QuantityPerUnit { get; set; }

    public decimal UnitPrice { get; set; }

    public long UnitsInStock { get; set; }

    public long UnitsOnOrder { get; set; }

    public bool Discontinued { get; set; }
}

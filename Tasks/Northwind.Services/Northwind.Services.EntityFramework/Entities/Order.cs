using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Northwind.Services.EntityFramework.Entities;

public class Order
{
    [Key]
    public long OrderID { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "EF Model")]
    [ForeignKey(nameof(OrderDetail.OrderID))]
    public virtual ICollection<OrderDetail>? OrderDetails { get; set; }

    public string? CustomerID { get; set; }

    public Customer? Customer { get; set; }

    public long? EmployeeID { get; set; }

    public Employee? Employee { get; set; }

    public DateTime? OrderDate { get; set; }

    public DateTime? RequiredDate { get; set; }

    public DateTime? ShippedDate { get; set; }

    public long? ShipVia { get; set; }

    public Shipper? Shipper { get; set; }

    public double Freight { get; set; }

    public string? ShipName { get; set; }

    public string? ShipAddress { get; set; }

    public string? ShipCity { get; set; }

    public string? ShipRegion { get; set; }

    public string? ShipPostalCode { get; set; }

    public string? ShipCountry { get; set; }
}

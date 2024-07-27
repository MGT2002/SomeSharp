using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Northwind.Services.EntityFramework.Entities;

public class Shipper
{
    [Key]
    public long ShipperID { get; set; }

    [ForeignKey(nameof(Order.ShipVia))]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "EF model")]
    public ICollection<Order>? Orders { get; set; }

    public string? CompanyName { get; set; }

    public string? Phone { get; set; }
}

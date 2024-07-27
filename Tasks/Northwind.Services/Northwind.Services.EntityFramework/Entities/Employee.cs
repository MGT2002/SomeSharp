using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Northwind.Services.EntityFramework.Entities;

public class Employee
{
    [Key]
    public long EmployeeID { get; set; }

    [ForeignKey(nameof(Order.EmployeeID))]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "EF model")]
    public ICollection<Order>? Orders { get; set; }

    public string? LastName { get; set; }

    public string? FirstName { get; set; }

    public string? Country { get; set; }
}

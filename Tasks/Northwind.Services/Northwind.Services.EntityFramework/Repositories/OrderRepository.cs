using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Northwind.Services.Repositories;
using Order = Northwind.Services.EntityFramework.Entities.Order;
using RepositoryOrder = Northwind.Services.Repositories.Order;

namespace Northwind.Services.EntityFramework.Repositories;

public sealed class OrderRepository : IOrderRepository
{
    private readonly Entities.NorthwindContext context;

    public OrderRepository(Entities.NorthwindContext context)
    {
        this.context = context;
    }

    public async Task<RepositoryOrder> GetOrderAsync(long orderId)
    {
        Order? o = await this.context.Orders
            .Where(o => o.OrderID == orderId)
            .Include(o => o.Customer)
            .Include(o => o.Employee)
            .Include(o => o.Shipper)
            .FirstOrDefaultAsync() ?? throw new OrderNotFoundException();

        List<Entities.OrderDetail> ods = await this.context.OrderDetails
            .Where(od => od.OrderID == orderId)
            .Include(od => od.Product).ThenInclude(od => od!.Supplier)
            .Include(od => od.Product).ThenInclude(od => od!.Category)
        .ToListAsync();

        return CreateRepositoryOrder(o, ods);
    }

    public async Task<IList<RepositoryOrder>> GetOrdersAsync(int skip, int count)
    {
        ValidateArguments(skip, count);

#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
        List<Order> orders = await this.context.Orders
        .Include(o => o.Customer)
        .Include(o => o.Employee)
        .Include(o => o.Shipper)
        .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Product).ThenInclude(p => p!.Supplier)
        .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Product).ThenInclude(p => p!.Category)
        .Skip(skip).Take(count).ToListAsync();
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

        return orders.Select(o =>
            CreateRepositoryOrder(o, o.OrderDetails!)).ToList();
    }

    public async Task<long> AddOrderAsync(RepositoryOrder order)
    {
        ThrowIfNull(order);

        return await this.AddOrderHandle(order);
    }

    public async Task RemoveOrderAsync(long orderId)
    {
        var order = await this.context.Orders
            .Include(o => o.OrderDetails)
            .FirstOrDefaultAsync(o => o.OrderID == orderId)
            ?? throw new OrderNotFoundException();

        _ = this.context.Remove(order);
        _ = await this.context.SaveChangesAsync();
    }

    public async Task UpdateOrderAsync(RepositoryOrder order)
    {
        ThrowIfNull(order);

        Order? o = await this.context.Orders
            .Include(o => o.OrderDetails)
            .FirstOrDefaultAsync(o => o.OrderID == order.Id)
            ?? throw new OrderNotFoundException();

        await this.RemoveOrderAsync(o.OrderID);
        _ = await this.AddOrderAsync(order);
    }

    private static RepositoryOrder CreateRepositoryOrder(
        Order o,
        ICollection<Entities.OrderDetail> ods)
    {
        var res = new RepositoryOrder(o.OrderID)
        {
            Customer = new Customer(new CustomerCode(o.Customer!.CustomerID!))
            {
                CompanyName = o.Customer!.CompanyName!,
            },
            Employee = new Employee(o.EmployeeID ?? default)
            {
                FirstName = o.Employee!.FirstName!,
                LastName = o.Employee!.LastName!,
                Country = o.Employee!.Country!,
            },
            OrderDate = (DateTime)o.OrderDate!,
            RequiredDate = (DateTime)o.RequiredDate!,
            ShippedDate = o.ShippedDate,
            Shipper = new Shipper(o.ShipVia ?? default)
            {
                CompanyName = o.Shipper!.CompanyName!,
            },
            Freight = o.Freight,
            ShipName = o.ShipName!,
            ShippingAddress = new ShippingAddress(
        o.ShipAddress!,
        o.ShipCity!,
        o.ShipRegion,
        o.ShipPostalCode!,
        o.ShipCountry!),
        };
        foreach (Entities.OrderDetail od in ods)
        {
            res.OrderDetails.Add(new OrderDetail(res)
            {
                Discount = od.Discount,
                Product = new Product(od.ProductID)
                {
                    Category = od.Product!.Category!.CategoryName!,
                    CategoryId = od.Product.CategoryID,
                    ProductName = od.Product!.ProductName!,
                    Supplier = od.Product!.Supplier!.CompanyName!,
                    SupplierId = od.Product.SupplierID,
                },
                Quantity = od.Quantity,
                UnitPrice = od.UnitPrice,
            });
        }

        return res;
    }

    private static void ValidateArguments(int skip, int count)
    {
        if (skip < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(skip));
        }

        if (count <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }
    }

    private static void ThrowIfNull<T>(T order)
    {
        ArgumentNullException.ThrowIfNull(order);
    }

    private async Task<long> AddOrderHandle(RepositoryOrder order)
    {
        _ = await this.context.Orders.FirstOrDefaultAsync(o => o.OrderID == order.Id)
            is not null ? throw new RepositoryException("The order already exists!") : 0;

        try
        {
            Order o = new Order()
            {
                OrderID = order.Id,
                CustomerID = order.Customer.Code.Code,
                EmployeeID = order.Employee.Id,
                OrderDate = order.OrderDate,
                RequiredDate = order.RequiredDate,
                ShippedDate = order.ShippedDate,
                ShipVia = order.Shipper.Id,
                Freight = order.Freight,
                ShipName = order.ShipName,
                ShipAddress = order.ShippingAddress.Address,
                ShipCity = order.ShippingAddress.City,
                ShipRegion = order.ShippingAddress.Region,
                ShipPostalCode = order.ShippingAddress.PostalCode,
                ShipCountry = order.ShippingAddress.Country,
            };

            var ods = order.OrderDetails.Select(od => new Entities.OrderDetail()
            {
                OrderID = order.Id,
                ProductID = od.Product.Id,
                Discount = od.Discount,
                Quantity = od.Quantity,
                UnitPrice = od.UnitPrice,
            }).ToList();

            _ = await this.context.Orders.AddAsync(o);
            await this.context.OrderDetails.AddRangeAsync(ods);
            _ = await this.context.SaveChangesAsync();

            return o.OrderID;
        }
        catch (Exception)
        {
            throw new RepositoryException();
        }
    }
}

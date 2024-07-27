using Microsoft.AspNetCore.Mvc;
using Northwind.Orders.WebApi.Models;
using Repo = Northwind.Services.Repositories;

namespace Northwind.Orders.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class OrdersController : ControllerBase
{
    private readonly Repo.IOrderRepository orderRepository;
    private readonly ILogger<OrdersController> logger;

    public OrdersController(Repo.IOrderRepository orderRepository, ILogger<OrdersController> logger)
    {
        this.orderRepository = orderRepository;
        this.logger = logger;
    }

    [HttpGet("{orderId}")]
    public async Task<ActionResult<FullOrder>> GetOrderAsync(long orderId)
    {
        try
        {
            Repo.Order order = await this.orderRepository.GetOrderAsync(orderId);

            FullOrder fo = new FullOrder()
            {
                Id = order.Id,
                Customer = new Customer()
                {
                    Code = order.Customer.Code.Code,
                    CompanyName = order.Customer.CompanyName,
                },
                Employee = new Employee()
                {
                    Id = order.Employee.Id,
                    FirstName = order.Employee.FirstName,
                    LastName = order.Employee.LastName,
                    Country = order.Employee.Country,
                },
                OrderDate = order.OrderDate,
                RequiredDate = order.RequiredDate,
                ShippedDate = order.ShippedDate,
                Shipper = new Shipper()
                {
                    Id = order.Shipper.Id,
                    CompanyName = order.Shipper.CompanyName,
                },
                Freight = order.Freight,
                ShipName = order.ShipName,
                ShippingAddress = new ShippingAddress()
                {
                    Address = order.ShippingAddress.Address,
                    City = order.ShippingAddress.City,
                    Region = order.ShippingAddress.Region,
                    PostalCode = order.ShippingAddress.PostalCode,
                    Country = order.ShippingAddress.Country,
                },
                OrderDetails = new List<FullOrderDetail>(order.OrderDetails.Count),
            };
            foreach (var od in order.OrderDetails)
            {
                fo.OrderDetails.Add(new FullOrderDetail()
                {
                    ProductId = od.Product.Id,
                    ProductName = od.Product.ProductName,
                    CategoryId = od.Product.CategoryId,
                    CategoryName = od.Product.Category,
                    SupplierId = od.Product.SupplierId,
                    SupplierCompanyName = od.Product.Supplier,
                    UnitPrice = od.UnitPrice,
                    Discount = od.Discount,
                });
            }

            return this.Ok(fo);
        }
        catch (Repo.OrderNotFoundException)
        {
            return this.NotFound();
        }
        catch (Exception e)
        {
            this.logger.LogError(e, "Failed to retrive order by {OrderId}", orderId);
            return this.StatusCode(500);
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BriefOrder>>> GetOrdersAsync(int? skip, int? count)
    {
        try
        {
            var orders = await this.orderRepository.GetOrdersAsync(skip ?? 0, count ?? 10);
            var res = orders.Select(o =>
            {
                var res = new BriefOrder()
                {
                    Id = o.Id,
                    CustomerId = o.Customer.Code.Code,
                    EmployeeId = o.Employee.Id,
                    OrderDate = o.OrderDate,
                    RequiredDate = o.RequiredDate,
                    ShippedDate = o.ShippedDate,
                    ShipperId = o.Shipper.Id,
                    Freight = o.Freight,
                    ShipName = o.ShipName,
                    ShipAddress = o.ShippingAddress.Address,
                    ShipCity = o.ShippingAddress.City,
                    ShipRegion = o.ShippingAddress.Region,
                    ShipPostalCode = o.ShippingAddress.PostalCode,
                    ShipCountry = o.ShippingAddress.Country,
                    OrderDetails = new List<BriefOrderDetail>(o.OrderDetails.Count),
                };
                foreach (var od in o.OrderDetails)
                {
                    res.OrderDetails.Add(new BriefOrderDetail()
                    {
                        ProductId = od.Product.Id,
                        UnitPrice = od.UnitPrice,
                        Quantity = od.Quantity,
                        Discount = od.Discount,
                    });
                }

                return res;
            }).ToList();

            return this.Ok(res);
        }
        catch (ArgumentException)
        {
            return this.BadRequest();
        }
        catch (Exception e)
        {
            this.logger.LogError(e, "Failed to retrive orders");
            return this.StatusCode(500);
        }
    }

    [HttpPost]
    public async Task<ActionResult<AddOrder>> AddOrderAsync(BriefOrder order)
    {
        ThrowIfNull(order);

        return await this.AddOrderHandle(order);
    }

    [HttpDelete("{orderId}")]
    public async Task<ActionResult> RemoveOrderAsync(long orderId)
    {
        try
        {
            await this.orderRepository.RemoveOrderAsync(orderId);

            return this.NoContent();
        }
        catch (Repo.OrderNotFoundException)
        {
            return this.NotFound();
        }
        catch (Exception e)
        {
            this.logger.LogError(e, "Failed to delete order by {OrderId}", orderId);
            return this.StatusCode(500);
        }
    }

    [HttpPut("orderId")]
    public async Task<ActionResult> UpdateOrderAsync(long orderId, BriefOrder order)
    {
        ThrowIfNull(order);

        return await this.UpdateHandle(orderId, order);
    }

    private static Repo.Order BriefOrderToRepoOrder(BriefOrder order)
    {
        Repo.Order o = new Repo.Order(order.Id)
        {
            Customer = new Repo.Customer(new Repo.CustomerCode(order.CustomerId)),
            Employee = new Repo.Employee(order.EmployeeId),
            OrderDate = order.OrderDate,
            RequiredDate = order.RequiredDate,
            ShippedDate = order.ShippedDate,
            ShipName = order.ShipName,
            Shipper = new Repo.Shipper(order.ShipperId),
            Freight = order.Freight,
            ShippingAddress = new Repo.ShippingAddress(
                order.ShipAddress,
                order.ShipCity,
                order.ShipRegion,
                order.ShipPostalCode,
                order.ShipCountry),
        };
        foreach (var od in order.OrderDetails)
        {
            o.OrderDetails.Add(new Repo.OrderDetail(o)
            {
                Discount = od.Discount,
                Quantity = od.Quantity,
                UnitPrice = od.UnitPrice,
                Product = new Repo.Product(od.ProductId),
            });
        }

        return o;
    }

    private static void ThrowIfNull<T>(T order)
    {
        ArgumentNullException.ThrowIfNull(order);
    }

    private async Task<ActionResult<AddOrder>> AddOrderHandle(BriefOrder order)
    {
        try
        {
            Repo.Order o = BriefOrderToRepoOrder(order);

            _ = await this.orderRepository.AddOrderAsync(o);

            return this.Ok(new AddOrder() { OrderId = o.Id, });
        }
        catch (Exception e)
        {
            this.logger.LogError(e, "Faild to add the order!");
            return this.StatusCode(500);
        }
    }

    private async Task<ActionResult> UpdateHandle(long orderId, BriefOrder order)
    {
        try
        {
            order.Id = orderId;
            Repo.Order o = BriefOrderToRepoOrder(order);
            await this.orderRepository.UpdateOrderAsync(o);

            return this.NoContent();
        }
        catch (Repo.OrderNotFoundException)
        {
            return this.NotFound();
        }
        catch (Exception e)
        {
            this.logger.LogError(e, "Failed to update order by id {OrderId}", orderId);
            return this.StatusCode(500);
        }
    }
}

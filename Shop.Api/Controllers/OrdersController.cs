using Microsoft.AspNetCore.Mvc;
using Shop.Api.Data;
using Shop.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ShopDbContext _context;

        public OrdersController(ShopDbContext context)
        {
            _context = context;
        }

        // GET /api/orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetAll()
        {
            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .ToListAsync();

            return Ok(orders);
        }

        // GET /api/orders/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Order>> GetById(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order is null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        // GET /api/orders/by-email/{email}
        [HttpGet("by-email/{email}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetByCustomerEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest("Email is required.");
            }

            var orders = await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .Include(o => o.Customer)
                .Where(o => o.Customer.Email == email)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return Ok(orders);
        }

        [HttpPost("{id:int}/pay")]
        public async Task<IActionResult> MarkAsPaid(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order is null)
            {
                return NotFound();
            }

            if (order.PaymentStatus == PaymentStatus.Paid)
            {
                return BadRequest("Order is already paid.");
            }

            order.PaymentStatus = PaymentStatus.Paid;
            order.Status = OrderStatus.Paid;

            order.InvoiceNumber ??= $"FV/{DateTime.UtcNow:yyyyMMdd}/{order.Id:D5}";

            await _context.SaveChangesAsync();

            return Ok(new
            {
                order.Id,
                order.PaymentStatus,
                order.Status,
                order.InvoiceNumber
            });
        }


        [HttpGet("stats")]
        public async Task<ActionResult> GetStats()
        {
            var totalOrders = await _context.Orders.CountAsync();
            var totalRevenue = await _context.Orders.SumAsync(o => o.TotalAmount);

            var last30Days = DateTime.UtcNow.AddDays(-30);

            var ordersLast30Days = await _context.Orders
                .Where(o => o.CreatedAt >= last30Days)
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Count = g.Count(),
                    Revenue = g.Sum(o => o.TotalAmount)
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            return Ok(new
            {
                totalOrders,
                totalRevenue,
                ordersLast30Days
            });
        }


        // POST /api/orders
        [HttpPost]
        public async Task<ActionResult<Order>> Create([FromBody] CreateOrderRequest request)
        {
            // Walidacja podstawowa
            if (request == null || request.Items == null || !request.Items.Any())
            {
                return BadRequest("Order must contain at least one item.");
            }

            if (string.IsNullOrWhiteSpace(request.CustomerEmail))
            {
                return BadRequest("Customer email is required.");
            }

            // Szukamy istniejącego klienta po emailu lub tworzymy nowego
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == request.CustomerEmail);

            if (customer is null)
            {
                customer = new Customer
                {
                    Email = request.CustomerEmail,
                    FirstName = request.CustomerFirstName ?? string.Empty,
                    LastName = request.CustomerLastName ?? string.Empty
                };

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
            }

            var order = new Order
            {
                CustomerId = customer.Id,
                CreatedAt = DateTime.UtcNow,
                Status = OrderStatus.AwaitingPayment,      // <-- tu
                PaymentStatus = PaymentStatus.Pending,     // <-- i tu
                Items = new List<OrderItem>()
            };

            decimal total = 0;


            foreach (var item in request.Items)
            {
                // Pobieramy produkt z bazy
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product is null)
                {
                    return BadRequest($"Product with id {item.ProductId} does not exist.");
                }

                if (item.Quantity <= 0)
                {
                    return BadRequest("Quantity must be greater than zero.");
                }

                var lineTotal = product.Price * item.Quantity;

                var orderItem = new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price,
                    LineTotal = lineTotal
                };

                total += lineTotal;
                order.Items.Add(orderItem);
            }

            order.TotalAmount = total;

            Coupon? coupon = null;
            if (!string.IsNullOrWhiteSpace(request.CouponCode))
            {
                var now = DateTime.UtcNow;

                coupon = await _context.Coupons
                    .FirstOrDefaultAsync(c =>
                        c.Code == request.CouponCode &&
                        c.IsActive &&
                        (c.ExpiresAt == null || c.ExpiresAt >= now) &&
                        (c.MaxUsageCount == null || c.UsedCount < c.MaxUsageCount));

                if (coupon is not null)
                {
                    var discount = order.TotalAmount * (coupon.DiscountPercent / 100m);
                    order.TotalAmount -= discount;

                    coupon.UsedCount++;
                    if (coupon.MaxUsageCount.HasValue && coupon.UsedCount >= coupon.MaxUsageCount.Value)
                    {
                        coupon.IsActive = false;
                    }
                }
            }


            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }

    }

    // DTO - obiekt do przyjmowania danych z requesta
    public class CreateOrderRequest
    {
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerFirstName { get; set; } = string.Empty;
        public string CustomerLastName { get; set; } = string.Empty;
        public List<OrderItemRequest> Items { get; set; } = new();
        public string? CouponCode { get; set; }   
    }

    public class OrderItemRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }



}

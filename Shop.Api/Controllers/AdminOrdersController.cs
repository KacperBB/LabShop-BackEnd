using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Api.Data;
using Shop.Api.Models;

namespace Shop.Api.Controllers
{
    [ApiController]
    [Route("api/admin/orders")]
    [Authorize(Roles = "Admin,Moderator")]
    public class AdminOrdersController : ControllerBase
    {
        private readonly ShopDbContext _context;

        public AdminOrdersController(ShopDbContext context)
        {
            _context = context;
        }

        // GET /api/admin/orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetAll()
        {
            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return Ok(orders);
        }

        public class UpdateOrderStatusRequest
        {
            public OrderStatus Status { get; set; }
        }

        // PUT /api/admin/orders/{id}/status
        [HttpPut("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateOrderStatusRequest request)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order is null)
            {
                return NotFound();
            }

            // (opcjonalnie) prosta walidacja przejść
            if (order.Status == OrderStatus.AwaitingPayment &&
                request.Status == OrderStatus.Shipped)
            {
                return BadRequest("Nie można wysłać zamówienia, które nie jest opłacone.");
            }

            order.Status = request.Status;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("stats")]
        public async Task<ActionResult> GetStats()
        {
            var totalOrders = await _context.Orders.CountAsync();
            var totalRevenue = await _context.Orders.SumAsync(o => o.TotalAmount);

            var last30Days = DateTime.UtcNow.AddDays(-30);

            var daily = await _context.Orders
                .Where(o => o.CreatedAt >= last30Days)
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new
                {
                    date = g.Key,
                    count = g.Count(),
                    revenue = g.Sum(o => o.TotalAmount)
                })
                .OrderBy(x => x.date)
                .ToListAsync();

            return Ok(new
            {
                totalOrders,
                totalRevenue,
                daily
            });
        }

    }
}

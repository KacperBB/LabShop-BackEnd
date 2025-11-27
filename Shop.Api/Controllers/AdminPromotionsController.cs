using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Api.Data;
using Shop.Api.Models;

namespace Shop.Api.Controllers
{
    [ApiController]
    [Route("api/admin/coupons")]
    [Authorize(Roles = "Admin,Moderator")]
    public class AdminCouponsController : ControllerBase
    {
        private readonly ShopDbContext _context;

        public AdminCouponsController(ShopDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Coupon>>> GetAll()
        {
            var coupons = await _context.Coupons
                .OrderByDescending(c => c.Id)
                .ToListAsync();

            return Ok(coupons);
        }

        public class GenerateCouponsRequest
        {
            public decimal DiscountPercent { get; set; }
            public DateTime? ExpiresAt { get; set; }
            public int? MaxUsageCount { get; set; }
            public int Count { get; set; } = 1;
        }

        [HttpPost("generate")]
        public async Task<ActionResult<IEnumerable<Coupon>>> Generate(GenerateCouponsRequest request)
        {
            if (request.Count <= 0 || request.Count > 100)
            {
                return BadRequest("Count musi być w zakresie 1–100.");
            }

            var coupons = new List<Coupon>();

            for (int i = 0; i < request.Count; i++)
            {
                var code = $"SHOP-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";

                var coupon = new Coupon
                {
                    Code = code,
                    DiscountPercent = request.DiscountPercent,
                    ExpiresAt = request.ExpiresAt,
                    MaxUsageCount = request.MaxUsageCount,
                    IsActive = true,
                    UsedCount = 0
                };

                coupons.Add(coupon);
                _context.Coupons.Add(coupon);
            }

            await _context.SaveChangesAsync();

            return Ok(coupons);
        }
    }
}

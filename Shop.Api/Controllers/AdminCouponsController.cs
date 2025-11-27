using Microsoft.AspNetCore.Mvc;
using Shop.Api.Data;
using Shop.Api.Models;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/admin/[controller]")]
public class CouponsController : ControllerBase
{
    private readonly ShopDbContext _context;
    public CouponsController(ShopDbContext context) => _context = context;

    public class CreateCouponRequest
    {
        public decimal DiscountPercent { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public int? MaxUsageCount { get; set; }
        public int Count { get; set; } = 1; // ile kuponów wygenerować
    }

    [HttpPost("generate")]
    public async Task<ActionResult<IEnumerable<Coupon>>> Generate(CreateCouponRequest request)
    {
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
                IsActive = true
            };

            coupons.Add(coupon);
            _context.Coupons.Add(coupon);
        }

        await _context.SaveChangesAsync();

        return Ok(coupons);
    }
}

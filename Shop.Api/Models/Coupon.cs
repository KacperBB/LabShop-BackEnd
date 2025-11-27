using System;

namespace Shop.Api.Models
{
    public class Coupon
    {
        public int Id { get; set; }

        public string Code { get; set; } = string.Empty;

        // Procent zniżki lub kwota – na początek procent
        public decimal DiscountPercent { get; set; }

        public DateTime? ExpiresAt { get; set; }

        public bool IsActive { get; set; } = true;

        public int? MaxUsageCount { get; set; }

        public int UsedCount { get; set; } = 0;
    }
}

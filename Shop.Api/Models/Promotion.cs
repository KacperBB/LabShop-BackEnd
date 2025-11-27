using System;
using System.Collections.Generic;

namespace Shop.Api.Models
{
    public class Promotion
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal DiscountPercent { get; set; }   // np. 10 = 10%
        public DateTime? StartsAt { get; set; }
        public DateTime? EndsAt { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }

}

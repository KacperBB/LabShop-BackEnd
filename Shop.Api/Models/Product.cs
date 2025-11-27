using System.Text.Json.Serialization;

namespace Shop.Api.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        // Jedna kategoria
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        // Wiele tagów (N-N przez ProductTag)
        public ICollection<ProductTag> ProductTags { get; set; } = new List<ProductTag>();

        // Promocja (opcjonalnie)
        public int? PromotionId { get; set; }
        public Promotion? Promotion { get; set; }
        [JsonIgnore]
        public ICollection<ProductReview> Reviews { get; set; } = new List<ProductReview>();
    }
}

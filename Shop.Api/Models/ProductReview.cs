namespace Shop.Api.Models
{
    public class ProductReview
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public string UserId { get; set; } = string.Empty;
        public AppUser User { get; set; } = null!;

        public int Rating { get; set; }          // 1–5
        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}

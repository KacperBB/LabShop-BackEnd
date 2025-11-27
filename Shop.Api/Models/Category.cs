using System.Text.Json.Serialization;

namespace Shop.Api.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;

        // 1 kategoria -> wiele produktów
        [JsonIgnore]
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}

using System.Text.Json.Serialization;

namespace Shop.Api.Models
{
    public class Tag
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        [JsonIgnore]
        public ICollection<ProductTag> ProductTags { get; set; } = new List<ProductTag>();
    }
}

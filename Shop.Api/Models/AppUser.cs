using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace Shop.Api.Models
{
    public class AppUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        [JsonIgnore]
        public ICollection<ProductReview> Reviews { get; set; } = new List<ProductReview>();
    }
}

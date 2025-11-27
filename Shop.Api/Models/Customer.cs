namespace Shop.Api.Models
{
    public class Customer
    {
        public int Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        // Nawigacja: klient może mieć wiele zamówień
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}

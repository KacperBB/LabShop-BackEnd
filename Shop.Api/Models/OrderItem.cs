namespace Shop.Api.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        // FK do Order
        public int OrderId { get; set; }
        public Order Order { get; set; }

        // FK do Product
        public int ProductId { get; set; }
        public Product Product { get; set; }

        // Ile sztuk produktu
        public int Quantity { get; set; }

        // Cena jednostkowa w momencie zakupu
        public decimal UnitPrice { get; set; }

        // Cena za tę linię (Quantity * UnitPrice)
        public decimal LineTotal { get; set; }
    }
}

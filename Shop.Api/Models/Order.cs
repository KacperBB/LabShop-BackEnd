using System;
using System.Collections.Generic;

namespace Shop.Api.Models
{
    public class Order
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        // Status zamówienia (cykl życia zamówienia)
        public OrderStatus Status { get; set; } = OrderStatus.AwaitingPayment;

        // Status płatności
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

        public string? PaymentMethod { get; set; }

        public string? InvoiceNumber { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

        public decimal TotalAmount { get; set; }
    }
}

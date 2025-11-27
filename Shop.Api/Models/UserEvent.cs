using System;

namespace Shop.Api.Models
{
    public class UserEvent
    {
        public int Id { get; set; }

        public string Type { get; set; } = string.Empty; // np. "ViewProduct", "AddToCart"

        public int? ProductId { get; set; }

        public string? SessionId { get; set; } // prosty identyfikator sesji z frontu

        public DateTime OccurredAt { get; set; }

        public int? DurationSeconds { get; set; } // np. czas oglądania detali produktu
    }
}

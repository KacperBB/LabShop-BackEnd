namespace Shop.Api.Models
{
    public enum PaymentStatus
    {
        Pending = 0,   // Oczekuje na płatność
        Paid = 1,      // Zapłacone
        Failed = 2     // Płatność nieudana / odrzucona
    }
}

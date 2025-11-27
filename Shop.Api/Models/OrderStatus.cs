namespace Shop.Api.Models
{
    public enum OrderStatus
    {
        AwaitingPayment = 0,   // Oczekuje na płatność
        Paid = 1,              // Zapłacony
        Processing = 2,        // W trakcie realizacji
        Shipped = 3,           // Wysłany
        Completed = 4          // Odebrany / Zakończony
    }
}

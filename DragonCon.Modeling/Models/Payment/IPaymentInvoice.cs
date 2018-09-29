using NodaTime;

namespace DragonCon.Modeling.Models.Payment
{
    public interface IPaymentInvoice
    {
        string Confirmation { get; set; }
        bool IsPaid { get; set; }
        string TicketId { get; set; }
        Instant Timestamp { get; set; }
    }
}
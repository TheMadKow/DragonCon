using NodaTime;

namespace DragonCon.Modeling.Models.Tickets
{
    public class PaymentInvoice
    {
        public string TicketId { get; set; }
        public bool IsPaid { get; set; }
        public string Confirmation { get;set; }
        public Instant Timestamp { get; set; }
    }
}
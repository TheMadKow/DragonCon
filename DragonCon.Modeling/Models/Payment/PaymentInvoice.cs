using NodaTime;

namespace DragonCon.Modeling.Models.Payment
{
    public class PaymentInvoice : IPaymentInvoice
    {
        public string TicketId { get; set; }
        public bool IsPaid { get; set; }
        public string Confirmation { get;set; }
        public string Notes { get; set; }
        public Instant Timestamp { get; set; }
    }
}
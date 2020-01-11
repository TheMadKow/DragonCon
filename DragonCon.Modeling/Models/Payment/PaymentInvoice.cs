using System.Collections.Generic;
using DragonCon.Modeling.Models.Tickets;
using NodaTime;

namespace DragonCon.Modeling.Models.Payment
{
    public class PaymentInvoice : IPaymentInvoice
    {
        public string Id { get; set; } = string.Empty;

        public string ParticipantId { get; set; } = string.Empty;
        public string ConventionId { get; set; } = string.Empty;


        public ITicket TicketCopy { get; set; }
        public Dictionary<string, double> Additional { get; set; } = new Dictionary<string, double>();
        public double TotalPayment { get; set; }

        
        public bool IsPaid { get; set; }
        public string Confirmation { get;set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;

        public Instant Timestamp { get; set; }
    }
}
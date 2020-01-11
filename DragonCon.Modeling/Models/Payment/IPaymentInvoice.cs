using System.Collections.Generic;
using DragonCon.Modeling.Models.Tickets;
using NodaTime;

namespace DragonCon.Modeling.Models.Payment
{
    public interface IPaymentInvoice
    {
        string Id { get; set; }

        string ParticipantId { get; set; }
        string ConventionId { get; set; }


        ITicket TicketCopy { get; set; }
        Dictionary<string, double> Additional { get; set; }
        double TotalPayment { get; set; }


        bool IsPaid { get; set; }
        string Confirmation { get; set; }
        string Notes { get; set; }

        Instant Timestamp { get; set; }
    }
}
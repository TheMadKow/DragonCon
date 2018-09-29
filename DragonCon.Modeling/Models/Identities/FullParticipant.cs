using System.Collections.Generic;
using DragonCon.Modeling.Models.Payment;
using DragonCon.Modeling.Models.Tickets;

namespace DragonCon.Modeling.Models.Identities
{
    public class FullParticipant : Participant, ISystemUser
    {
        public string EmailAddress { get; set; }
        public List<string> Roles { get; set; } 
        public bool IsAllowingPromotions { get; set; }
        public List<PaymentInvoice> TicketPayments { get;set; }
    }
}
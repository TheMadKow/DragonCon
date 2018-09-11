using System.Collections.Generic;
using DragonCon.Modeling.Models.Tickets;

namespace DragonCon.Modeling.Models.Identities
{
    public class FullParticipant : Participant
    {
        public string EmailAddress { get; set; }
        public List<string> Roles { get; set; } 
        public string HashedPassword { get; set; }
        public string Token { get; set; }
        public bool IsAllowingPromotions { get; set; }
        public List<PaymentInvoice> TicketPayments { get;set; }
    }
}
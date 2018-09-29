using DragonCon.Modeling.Models.Payment;
using DragonCon.Modeling.Models.Tickets;

namespace DragonCon.Modeling.Models.Identities
{
    public class ParticipantLimited : Participant
    {
        public string LimitedToConventionId { get; set; }

        public PaymentInvoice PaymentInvoice { get; set; }
    }
}
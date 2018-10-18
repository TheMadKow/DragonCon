using System;
using DragonCon.Modeling.Models.Payment;
using DragonCon.Modeling.Models.Tickets;

namespace DragonCon.Modeling.Models.Identities
{
    public class LimitedParticipant : IParticipant
    {
        public string Id { get; set;}
        public DateTimeOffset DayOfBirth { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }

        public string LimitedToConventionId { get; set; }
        public PaymentInvoice PaymentInvoice { get; set; }
    }
}
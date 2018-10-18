using System;
using System.Collections.Generic;
using DragonCon.Modeling.Models.Payment;
using DragonCon.Modeling.Models.Tickets;
using Raven.Identity;

namespace DragonCon.Modeling.Models.Identities
{
    public class FullParticipant : IdentityUser, IParticipant
    {
        public bool IsAllowingPromotions { get; set; }
        public Dictionary<string, PaymentInvoice> ConIdAndPayment { get;set; }
        public DateTimeOffset DayOfBirth { get; set; }
        public string EmailAddress { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

    }
}
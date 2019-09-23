using System;
using DragonCon.Modeling.Models.Payment;
using DragonCon.Modeling.Models.Tickets;
using NodaTime;

namespace DragonCon.Modeling.Models.Identities
{
    public class LimitedParticipant : IParticipant
    {
        public string Id { get; set;}
        public LocalDate DayOfBirth { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public string FirstName
        {
            get
            {
                try
                {
                    var breakName = FullName.Split(new []{' '}, StringSplitOptions.RemoveEmptyEntries);
                    return breakName[0].Trim();
                }
                catch
                {
                    return FullName;
                }
            }
        }

        public string FullName { get; set; }

        public string LimitedToConventionId { get; set; }
        public string CreatedById { get; set; }

        public PaymentInvoice PaymentInvoice { get; set; }
    }
}
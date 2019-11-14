using System;
using System.Collections.Generic;
using DragonCon.Modeling.Helpers;
using DragonCon.Modeling.Models.Payment;
using DragonCon.Modeling.Models.Tickets;
using NodaTime;

namespace DragonCon.Modeling.Models.Identities
{
    public class ShortTermParticipant : IParticipant
    {
        public string Id { get; set;} = string.Empty;
        public LocalDate DayOfBirth { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;

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

        public string FullName { get; set; } = string.Empty;
        public string ActiveConventionTerm { get; set; } = string.Empty;
        public string CreatedById { get; set; } = string.Empty;
        public PaymentInvoice PaymentInvoice { get; set; }

        public IList<ConventionRoles> ActiveConventionRoles { get; } = new List<ConventionRoles>();

        public void AddRole(ConventionRoles role)
        {
            if (ActiveConventionRoles.Missing(role))
                ActiveConventionRoles.Add(role);
        }
        public bool HasRole(ConventionRoles role)
        {
            return ActiveConventionRoles.Contains(role);
        }
        public void RemoveRole(ConventionRoles role)
        {
            if (ActiveConventionRoles.Contains(role))
                ActiveConventionRoles.Remove(role);
        }
    }
}
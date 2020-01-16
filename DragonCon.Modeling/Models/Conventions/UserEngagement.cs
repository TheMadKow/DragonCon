using System.Collections.Generic;
using DragonCon.Modeling.Helpers;
using DragonCon.Modeling.Models.Identities;
using DragonCon.Modeling.Models.Payment;
using NodaTime;

namespace DragonCon.Modeling.Models.Conventions
{
    public class UserEngagement : IUserEngagement
    {
        public string Id { get; set; }
        public string ConventionId { get; set; }
        public string ConventionStartDate { get; set; }
        public string CreatorId { get; set; }
        public string ParticipantId { get; set; }
        public bool IsLongTerm { get; set; }
        public bool IsShortTerm => !IsLongTerm;
        public IPaymentInvoice Payment { get; set; } = new PaymentInvoice();
        public List<string> EventIds { get; set; } = new List<string>();
        public List<string> SuggestedEventIds { get; } = new List<string>();
        public List<ConventionRoles> Roles { get; set; } = new List<ConventionRoles>();
        public string RoleDescription { get; set; } = string.Empty;
        public Instant CreatedOn { get; set; } = SystemClock.Instance.GetCurrentInstant();

        public void AddRole(ConventionRoles role)
        {
            if (Roles.Missing(role))
                Roles.Add(role);
        }

        public void RemoveRole(ConventionRoles role)
        {
            if (Roles.Contains(role))
                Roles.Remove(role);
        }

        public bool HasRole(ConventionRoles role)
        {
            return Roles.Contains(role);
        }
    }
}
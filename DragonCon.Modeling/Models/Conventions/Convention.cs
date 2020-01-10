using System.Collections.Generic;
using DragonCon.Modeling.Helpers;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Identities;
using DragonCon.Modeling.Models.Payment;
using NodaTime;

namespace DragonCon.Modeling.Models.Conventions
{
    public class ConventionEngagement : IConventionEngagement
    {
        public string Id { get; set; }
        public string ConventionId { get; set; }
        public string CreatorId { get; set; }
        public string ParticipantId { get; set; }
        public bool IsLongTerm { get; set; }
        public IPaymentInvoice Payment { get; set; } = new PaymentInvoice();
        public List<string> EventIds { get; set; } = new List<string>();
        public List<string> SuggestedEventIds { get; } = new List<string>();
        public List<ConventionRoles> Roles { get; set; } = new List<ConventionRoles>();
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

    public class Convention
    {
        public string Id { get; set; } = null;
        public string Name { get; set; } = string.Empty;
        
        public TimeSlotStrategy TimeStrategy { get; set; } 
        public List<string> DayIds { get; set; } = new List<string>();
        public List<string> TicketIds { get; set; }= new List<string>();
        public List<string> HallIds { get; set; }= new List<string>();
        
        public Instant CreateTimeStamp { get; set; }
        public Instant UpdateTimeStamp { get; set; }
        
        public string TagLine { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public ConventionSettings Settings { get; set; } = new ConventionSettings();
    }

    public class ConventionSettings
    {
        public bool AllowManagementTests { get; set; } = false;
        public bool AllowEventsRegistration { get; set; } = false;
        public bool AllowEventsSuggestions { get; set; } = false;
        public bool AllowPayments { get; set; } = false;
    }
}

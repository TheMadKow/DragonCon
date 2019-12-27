using System.Collections.Generic;
using DragonCon.Modeling.Helpers;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Identities;
using DragonCon.Modeling.Models.Payment;
using NodaTime;

namespace DragonCon.Modeling.Models.Conventions
{
    public interface IConventionEngagement
    {
        string ConventionId { get; }
        string ParticipantId { get; }
        IPaymentInvoice Payment { get; }
        bool IsLongTerm { get;}
        List<string> EventIds { get; }
        List<ConventionRoles> Roles { get; }
        Instant CreatedOn { get; }
    }

    public class ConventionEngagement : IConventionEngagement
    {
        public string Id { get; set; }
        public string ConventionId { get; set; }
        public string ParticipantId { get; set; }
        public bool IsLongTerm { get; set; }
        public IPaymentInvoice Payment { get; set; } = new PaymentInvoice();
        public List<string> EventIds { get; set; } = new List<string>();
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
        
        public List<PhoneRecord> PhoneBook { get;set; } = new List<PhoneRecord>();
        public Dictionary<string, string> Metadata { get;set; } = new Dictionary<string, string>();

        
        public Instant CreateTimeStamp { get; set; }
        public Instant UpdateTimeStamp { get; set; }
        
        public string TagLine { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public ConventionSettings Settings { get; set; } = new ConventionSettings();
    }

    public class ConventionSettings
    {
        public bool AllowEventsRegistration { get; set; } = false;
        public bool AllowEventsRegistrationChanges { get; set; } = false;
        public bool AllowEventsSuggestions { get; set; } = false;
        public bool AllowPayments { get; set; } = false;
        public bool AllowPaymentChanges { get; set; } = false;

    }
}

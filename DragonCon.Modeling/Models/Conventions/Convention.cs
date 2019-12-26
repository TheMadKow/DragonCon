using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Identities;
using DragonCon.Modeling.Models.Payment;
using NodaTime;

namespace DragonCon.Modeling.Models.Conventions
{
    public interface IConventionEngagement
    {
        string ConventionId { get; set; }
        string ParticipantId { get; set; }
        IPaymentInvoice Payment { get; set; }
        bool IsLongTerm { get; set; }
        List<string> EventIds { get; set; }
    }

    public class ConventionEngagement : IConventionEngagement
    {
        public string Id { get; set; }
        public string ConventionId { get; set; }
        public string ParticipantId { get; set; }
        public IPaymentInvoice Payment { get; set; }
        public bool IsLongTerm { get; set; }
        public List<string> EventIds { get; set; } = new List<string>();
    }

    public class Convention
    {
        public string Id { get; set; } = null;
        public string Name { get; set; } = string.Empty;
        
        public TimeSlotStrategy TimeStrategy { get; set; } 
        public List<string> DayIds { get; set; } = new List<string>();
        public List<string> TicketIds { get; set; }= new List<string>();
        public List<string> HallIds { get; set; }= new List<string>();
        public List<string> EventIds { get; set; }= new List<string>();
        
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

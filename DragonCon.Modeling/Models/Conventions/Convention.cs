using System.Collections.Generic;
using DragonCon.Modeling.Models.Common;
using NodaTime;

namespace DragonCon.Modeling.Models.Conventions
{
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

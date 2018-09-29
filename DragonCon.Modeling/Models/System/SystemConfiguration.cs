using System.Collections.Generic;

namespace DragonCon.Modeling.Models.System
{
    public class SystemConfiguration
    {
        public string ActiveConventionId { get; set; }
     
        public bool AllowEventsRegistration { get; set; }
        public bool AllowEventRegistrationChanges { get; set; }
        public bool AllowEventSuggestions { get; set; }
        public bool AllowPayments { get;set; }
    }
}

using System.Collections.Generic;

namespace DragonCon.Modeling.Models.System
{
    public class SystemConfiguration
    {
        public string ActiveConventionId { get; set; }
     
        public bool AllowEventsRegistration { get; set; }
        public bool AllowEventsRegistrationChanges { get; set; }
        public bool AllowEventsSuggestions { get; set; }
        public bool AllowPayments { get;set; }
    }
}

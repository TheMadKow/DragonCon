using System.Collections.Generic;

namespace DragonCon.Modeling.Models.System
{
    public class SystemConfiguration
    {
        public static string Id = "Dragon-System-Configuration";
        public string ActiveConventionId { get; set; } = "";
        public bool AllowEventsRegistration { get; set; } = false;
        public bool AllowEventsRegistrationChanges { get; set; } = false;
        public bool AllowEventsSuggestions { get; set; } = false;
        public bool AllowPayments { get; set; } = false;
        public bool AllowPaymentChanges { get; set; } = false;
    }
}

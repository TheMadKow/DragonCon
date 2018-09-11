using System.Collections.Generic;

namespace DragonCon.Modeling.Models.System
{
    public class SystemConfiguration
    {
        public string ActiveConventionId { get; set; }
        
        public bool AllowEventsRegistration { get; set; }
        public bool AllowEventRegistrationUpdates { get; set; }
        public bool AllowEventRequests { get; set; }


        public List<Menu> Menus { get; set; }
    }
}

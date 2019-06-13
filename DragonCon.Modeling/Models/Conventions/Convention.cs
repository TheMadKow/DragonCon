using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NodaTime;

namespace DragonCon.Modeling.Models.Conventions
{
    public class Convention
    {
        public string Id { get; set; }
        public string Name { get; set; }
        
        public List<string> DayIds { get; set; }
        public List<string> TicketIds { get; set; }
        public List<string> HallIds { get; set; }
        public List<string> EventIds { get; set; }

        public List<PhoneRecord> PhoneBook { get;set; }
        public Dictionary<string, string> Metadata { get;set; }
        
        public Instant CreateTimeStamp { get; set; }
        public Instant UpdateTimeStamp { get; set; }
    }
}

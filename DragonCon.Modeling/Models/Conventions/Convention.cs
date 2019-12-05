using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Identities;
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
        public List<string> EventIds { get; set; }= new List<string>();
        
        public List<PhoneRecord> PhoneBook { get;set; } = new List<PhoneRecord>();
        public Dictionary<string, string> Metadata { get;set; } = new Dictionary<string, string>();

        
        public Instant CreateTimeStamp { get; set; }
        public Instant UpdateTimeStamp { get; set; }
        
        public string TagLine { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
    }
}

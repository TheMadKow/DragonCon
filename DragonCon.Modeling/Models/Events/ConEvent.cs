using System.Collections.Generic;
using DragonCon.Modeling.Models.Common;

namespace DragonCon.Modeling.Models.Events
{
    public class ConEvent : IConEvent
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ConventionDayId { get;set; }
        public string ActivityId { get; set; }
        public string SystemId { get;set; }

        public string GameMasterId { get; set; }
        public List<string> HelperIds { get; set; }
        public List<string> ParticipantIds { get;set; }
        public bool HasBeenRevised { get; set; }

        public EventStatus Status { get; set; }
        public TimeSlot TimeSlot { get;set; }
        public AgeRestriction Age { get; set; }
        public SizeRestriction Size { get;set; }

        public List<string> Tags { get;set; }
        public string TableId { get; set; }
        
        public string Description { get; set; }
        public string Requests { get;set; }

        public List<string> Changes { get;set; }
    }
}

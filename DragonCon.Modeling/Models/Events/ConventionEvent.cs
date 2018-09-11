using System.Collections.Generic;
using DragonCon.Modeling.Models.Common;

namespace DragonCon.Modeling.Models.Events
{
    public class ConventionEvent
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ConventionDayId { get;set; }
        public string ActivityId { get; set; }
        public string ManagerId { get; set; }
        public IList<string> HelpersId { get; set; }
        public List<string> ParticipantIds { get;set; }
        public bool HasBeenRevised { get; set; }

        public string SystemId { get;set; }
        public EventStatus Status { get; set; }
        public AgeRestriction Age { get; set; }
        public SizeRestriction Size { get;set; }

        public List<string> Tags { get;set; }

        public List<string> HistoryIds {get; set;}
        public string HallId  { get;set; }
        public string Table { get; set; }

        public TimeSlot TimeSlot { get;set; }
        public string Description { get; set; }
        public string Comments { get;set; }
    }
}

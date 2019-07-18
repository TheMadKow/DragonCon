using System.Collections.Generic;
using DragonCon.Modeling.Models.Common;
using NodaTime;

namespace DragonCon.Modeling.Models.Events
{
    public class ConEvent : IConEvent
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ConventionId { get; set; }
        public string ConventionDayId { get;set; }
        public string ActivityId { get; set; }
        public string SystemId { get;set; }

        public string GameMasterId { get; set; }
        public List<string> HelperIds { get; set; }
        public List<string> ParticipantIds { get;set; }

        public EventStatus Status { get; set; }
        public TimeSlot TimeSlot { get;set; }
        public AgeRestriction Age { get; set; }
        public SizeRestriction Size { get;set; }

        public List<string> Tags { get;set; }
        public string HallId { get; set; }
        public int? Table { get; set; }
        
        public string Description { get; set; }
        public string SpecialRequests { get;set; }

        public List<EventHistory> StatusHistory { get;set; }
    }

    public class EventHistory
    {
        public string UserId { get; set; }
        public Instant Timestamp { get; set; }
        public EventStatus Status { get; set; }
    }
}

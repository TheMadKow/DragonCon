using System.Collections.Generic;
using NodaTime;

namespace DragonCon.Modeling.Models.Tracking
{
    public class EventsTracking
    {
        public string Id { get; set; }
        public string EventId { get; set; }
        public Instant TimeStamp { get; set; }
        
        public string ParticipantId { get;set; }
        public string ExecutorId { get;set; }

        public bool IsSelf => ParticipantId == ExecutorId;
        public EventTrackAction EventAction { get;set; }
    }
}

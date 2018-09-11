using System.Collections.Generic;
using NodaTime;

namespace DragonCon.Modeling.Models.Tracking
{
    public class EventsTracking
    {
        public string ConventionId { get; set; }
        public Instant TimeStamp { get; set; }
        public string ParticipantId { get;set; }

        public Dictionary<string, EventTrackAction> EventAndAction { get;set; }
    }
}

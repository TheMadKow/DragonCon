using System.Collections.Generic;
using NodaTime;

namespace DragonCon.Modeling.Models.Events
{
    public class EventChange
    {
        public EventChange()
        {
            TimeStamp = SystemClock.Instance.GetCurrentInstant();
        }

        public string Id { get; set; }
        public string ExecutorId { get; set; }
        public Instant TimeStamp { get; set; }
        public List<FieldChange> Changes {get; set; }
        public bool IsAnnouncement {get;set;}
        public bool IsCreationChange { get; set; }

        public class FieldChange
        {
            public string Name { get; set; }
            public string PreviousAsJson { get; set; }
            public string CurrentAsJson { get; set; }
        }
    }
}
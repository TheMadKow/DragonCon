using System.Collections.Generic;
using NodaTime;

namespace DragonCon.Modeling.Models.Events
{
    public class EventChange
    {
        public string Id { get; set; }
        public string ExecutorId { get; set; }
        public Instant TimeStamp { get; set; }
        public List<FieldChange> Changes {get; set; }

        public class FieldChange
        {
            public string FieldName { get; set; }
            public string PreviousAsJson { get; set; }
            public string CurrentAsJson { get; set; }
        }
    }
}
using System.Collections.Generic;
using NodaTime;

namespace DragonCon.Logical.Factories
{
    public class TimeSlotOptions
    {
        public IList<LocalTime> StartTime { get;set; }
        public List<double> Durations { get; set; }
    }
}
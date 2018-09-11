using System.Collections.Generic;
using NodaTime;

namespace DragonCon.Logic
{
    public class TimeSlotOptions
    {
        public IList<LocalTime> StartTime { get;set; }
        public List<double> Durations { get; set; }
    }
}
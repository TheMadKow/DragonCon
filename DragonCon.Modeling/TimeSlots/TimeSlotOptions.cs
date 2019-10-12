using System.Collections.Generic;
using NodaTime;

namespace DragonCon.Modeling.TimeSlots
{
    public class TimeSlotOptions
    {
        public Dictionary<LocalTime, List<double>> StartTimeAndDurations { get; set; }
    }
}
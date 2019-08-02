using System.Collections.Generic;
using NodaTime;

namespace DragonCon.Logical.Factories
{
    public class TimeSlotOptions
    {
        public Dictionary<LocalTime, List<double>> StartTimeAndDurations { get; set; }
    }
}
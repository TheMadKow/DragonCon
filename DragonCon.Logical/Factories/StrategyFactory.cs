using System;
using System.Collections.Generic;
using DragonCon.Modeling.Models.Convention;
using NodaTime;

namespace DragonCon.Logical.Factories
{
    public class StrategyFactory
    {
        public TimeSlotOptions TimeSlots(LocalTime start, LocalTime end, TimeSlotStrategy strategy)
        {
            switch (strategy)
            {
                case TimeSlotStrategy.Exact246Windows:
                    return GenerateExact246Windows(start, end);
            }

            throw new Exception("Unknown Time Slots Strategy");
        }

        private TimeSlotOptions GenerateExact246Windows(LocalTime start, LocalTime end)
        {
            var options = new TimeSlotOptions
            {
                Durations = new List<double>()
                {
                    2d, 4d, 6d
                }, 
                StartTime = new List<LocalTime>()
            };

            var fromHere = new LocalTime(start.Hour, start.Minute, start.Second);
            while (fromHere < end)
            {
                if (fromHere < end)
                    options.StartTime.Add(fromHere);
                
                fromHere = fromHere.PlusHours(2);
            }

            return options;
        }
    }
}

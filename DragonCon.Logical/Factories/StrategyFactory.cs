using System;
using System.Collections.Generic;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.TimeSlots;
using NodaTime;

namespace DragonCon.Logical.Factories
{
    public class StrategyFactory : IStrategyFactory
    {
        public TimeSlotOptions GenerateTimeSlots(LocalTime start, LocalTime end, TimeSlotStrategy strategy)
        {
            switch (strategy)
            {
                case TimeSlotStrategy.StartEvery2Hours_Duration246Windows:
                    return StartEvery2Hours_Duration246Windows(start, end);
                case TimeSlotStrategy.StartEvery2Hours_Duration90MinutesWindows:
                    return StartEvery2Hours_Duration90MinutesWindows(start, end);
            }

            throw new Exception("Unknown Time Slots Strategy");
        }


        private TimeSlotOptions StartEvery2Hours_Duration90MinutesWindows(LocalTime start, LocalTime end)
        {
            var options = new TimeSlotOptions();
            options.StartTimeAndDurations = new Dictionary<LocalTime, List<double>>();

            var fromHere = new LocalTime(start.Hour, start.Minute, start.Second);
            var fromHereHour = fromHere.Hour;
            var durations = new List<double> { 1.5d, 3.5d, 5.5d };

            while (fromHereHour < end.Hour)
            {
                if (fromHere.Hour + 5.5 > end.Hour)
                    durations.Remove(5.5);
                if (fromHere.Hour + 3.5 > end.Hour)
                    durations.Remove(3.5);
                if (fromHere.Hour + 1.5 > end.Hour)
                    durations.Remove(1.5);

                options.StartTimeAndDurations.Add(fromHere, new List<double>(durations));

                fromHere = fromHere.PlusHours(2);
                fromHereHour += 2;
            }

            return options;
        }
        private TimeSlotOptions StartEvery2Hours_Duration246Windows(LocalTime start, LocalTime end)
        {
            var options = new TimeSlotOptions();
            options.StartTimeAndDurations = new Dictionary<LocalTime, List<double>>();
            
            var fromHere = new LocalTime(start.Hour, start.Minute, start.Second);
            var fromHereHour = fromHere.Hour;
            var durations = new List<double> {2d, 4d, 6d};

            while (fromHereHour < end.Hour)
            {
                if (fromHere.Hour + 6 > end.Hour)
                    durations.Remove(6);
                if (fromHere.Hour + 4 > end.Hour)
                    durations.Remove(4);
                if (fromHere.Hour + 2 > end.Hour)
                    durations.Remove(2);

                options.StartTimeAndDurations.Add(fromHere, new List<double>(durations));
                
                fromHere = fromHere.PlusHours(2);
                fromHereHour += 2;
            }

            return options;
        }
    }
}

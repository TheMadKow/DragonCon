using DragonCon.Modeling.Models.Common;
using NodaTime;

namespace DragonCon.Modeling.TimeSlots
{
    public interface IStrategyFactory
    {
        TimeSlotOptions GenerateTimeSlots(LocalTime start, LocalTime end, TimeSlotStrategy strategy);
    }
}

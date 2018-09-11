using NodaTime;

namespace DragonCon.Modeling.Models.Common
{
    public class TimeSlot
    {
        public LocalTime From { get; set; }
        public LocalTime To { get; set; }
        public Period Span => To - From;
    }
}

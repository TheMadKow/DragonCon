using NodaTime;

namespace DragonCon.Logical.Requests
{
    public class TimeSlotRequest
    {
        public LocalDate Date { get;set; }
        public LocalTime StartTime { get;set; }
        public int Duration { get; set; }
    }
}
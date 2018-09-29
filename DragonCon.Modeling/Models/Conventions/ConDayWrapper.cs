using DragonCon.Modeling.Models.Common;
using NodaTime;

namespace DragonCon.Modeling.Models.Conventions
{
    public class ConDayWrapper : Wrapper<ConDay>
    {
        public ConDayWrapper() : base() { }

        public ConDayWrapper(ConDay model) : base(model) { }


        public string Id
        {
            get => Model.Id;
            set => Model.Id = value;
        }

        public LocalDate Date
        {
            get => Model.Date; 
            set => Model.Date = value; 
        }

        public LocalTime StartTime
        {
            get => Model.StartTime;
            set => Model.StartTime = value;
        }

        public LocalTime EndTime
        {
            get => Model.EndTime;
            set => Model.EndTime = value;
        }

        public TimeSlotStrategy TimeSlotStrategy
        {
            get => Model.TimeSlotStrategy;
            set => Model.TimeSlotStrategy = value;
        }
    }
}
using System.Collections.Generic;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.Events;
using DragonCon.Modeling.Models.HallsTables;
using DragonCon.Modeling.Models.Identities;

namespace DragonCon.Features.Shared
{
    public class DisplayEventsViewModel
    {
        public class Filters
        {
            public string StartTime { get; set; } = string.Empty;
            public string ActivitySelection { get; set; } = string.Empty;
            public bool HideCompleted { get; set; } = false;
            public bool HideTaken { get; set; } = false;
        }

        public IDisplayPagination Pagination { get; set; } 
        public Filters ActiveFilters { get; set; } = new Filters();

        public List<DisplayEventViewModel> Events { get; set; } = new List<DisplayEventViewModel>();
    }


    public class DisplayEventViewModel
    {
        public DisplayEventViewModel(EventWrapper eventWrapper, int seats, int takenSeats)
        {
            SeatsCapacity = seats;
            SeatsTaken = takenSeats;
            SeatsAvailable = SeatsCapacity - takenSeats;

            Id = eventWrapper.Inner.Id;
            Name = eventWrapper.Inner.Name;
            IsFree = eventWrapper.Inner.IsFree;
            ExtraCharge = eventWrapper.Inner.ExtraCharge;
            TimeSlot = eventWrapper.Inner.TimeSlot;
            TimeSlot = eventWrapper.Inner.TimeSlot;
            HallTable = eventWrapper.Inner.HallTable;
            Description = eventWrapper.Inner.Description;

            Day = eventWrapper.Day;
            Activity = eventWrapper.Activity;
            SubActivity = eventWrapper.SubActivity;
            GameHosts = eventWrapper.GameHosts;
            AgeGroup = eventWrapper.AgeGroup;
            Hall = eventWrapper.Hall;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public Day Day { get; set; }
        public Activity Activity { get; set; }
        public Activity SubActivity { get; set; }
        public IList<IParticipant> GameHosts { get; set; }
        public TimeSlot TimeSlot { get; set; }
        public bool IsFree { get; set; }
        public double? ExtraCharge { get; set; }

        public AgeGroup AgeGroup { get; set; }
        public Hall Hall { get; set; }
        public int? HallTable { get; set; }
        public string Description { get; set; }

        public int SeatsCapacity { get; set; } = 0;
        public int SeatsTaken { get; set; } = 0;
        public int SeatsAvailable { get; set; } = 0;


    }
}

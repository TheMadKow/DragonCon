using System.Collections.Generic;
using System.Linq;
using DragonCon.Modeling.Helpers;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.Events;
using NodaTime;

namespace DragonCon.Features.Management.Dashboard
{
    public class ConventionStatisticsViewModel
    {
        #region Payments
        public Dictionary<string, int> PaymentCompleted { get; set; } = new Dictionary<string, int>();

        public int PaymentCompletedCount => PaymentCompleted.Sum(y => y.Value);
        public int PaymentPendingCount = 0;

        public void AddPayment(string ticketName, bool isPaid)
        {
            if (isPaid)
            {
                if (PaymentCompleted.MissingKey(ticketName))
                    PaymentCompleted[ticketName] = 0;
                PaymentCompleted[ticketName]++;
            }
            else
            {
                PaymentPendingCount++;
            }

        }
        #endregion

        #region Events
        public void AddEventSeats(Dictionary<string,Day> days, Event conEvent, string majorActivity)
        {
            if (EventSeats == null)
                EventSeats = new List<EventSeatViewModel>();

            var vm = EventSeats.FirstOrDefault(x => x.EventId == conEvent.Id);
            if (vm == null)
            {
                vm = new EventSeatViewModel
                {
                    EventId = conEvent.Id
                };
                EventSeats.Add(vm);
            }

            var date = days.ContainsKey(conEvent.ConventionDayId)
                ? days[conEvent.ConventionDayId].Date
                : new LocalDate(1, 1, 1);
            var time = conEvent.TimeSlot != null ? conEvent.TimeSlot.From : new LocalTime(0, 0);
            var localDateTime = new LocalDateTime(
                date.Year,
                date.Month,
                date.Day,
                time.Hour,
                time.Minute);

            vm.Major = majorActivity;
            vm.EventTime = localDateTime;
            vm.TotalSeats = (int) conEvent.Size.Max;
        }

        public void AddEventTakenSeat(string eventId)
        {
            var seats = EventSeats.FirstOrDefault(x => x.EventId == eventId);
            seats.TakenSeats++;
        }

        public List<EventSeatViewModel> EventSeats { get; set; } = new List<EventSeatViewModel>();
        public int TotalEventSeats => EventSeats.Sum(y => y.TotalSeats);
        public int TotalEventTakenSeats => EventSeats.Sum(y => y.TakenSeats);
        public int TotalEventFreeSeats => EventSeats.Sum(y => y.FreeSeats);


        #endregion

        public Modeling.Models.Conventions.Convention SelectedConvention { get; set; }


        public int TotalLongTermParticipants { get; set; }
        public int TotalShortTermParticipants { get; set; }
        public int TotalParticipants => TotalLongTermParticipants + TotalShortTermParticipants;
    }

    public class EventSeatViewModel
    {
        public string EventId { get; set; }
        public LocalDateTime EventTime { get; set; }
        public int TotalSeats { get; set; }
        public int TakenSeats { get; set; }
        public int FreeSeats => TotalSeats - TakenSeats;

        public string Major { get; internal set; }
    }
}

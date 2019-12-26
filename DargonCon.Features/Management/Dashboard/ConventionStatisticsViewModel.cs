using System.Collections.Generic;
using System.Linq;
using DragonCon.Modeling.Helpers;
using NodaTime;

namespace DragonCon.Features.Management.Dashboard
{
    public class ConventionStatisticsViewModel
    {
        public void AddTicketCount(string ticketName, int count)
        {
            if (TicketRegistration == null)
                TicketRegistration = new Dictionary<string, int>();

            TicketRegistration.Add(ticketName, count);
        }
        public void AddEventCount(string major, LocalDateTime localStart, int count)
        {
            if (EventTimeRegistration == null)
                EventTimeRegistration = new Dictionary<string, Dictionary<LocalDateTime, int>>();

            if (EventTimeRegistration.MissingKey(major))
                EventTimeRegistration.Add(major, new Dictionary<LocalDateTime, int>());

            EventTimeRegistration[major].Add(localStart, count);
        }

        public Modeling.Models.Conventions.Convention SelectedConvention { get; set; }
        public Dictionary<string, string> AllConventions { get; set; } = new Dictionary<string, string>();

        public Dictionary<string, int> TicketRegistration { get; set; } = new Dictionary<string, int>();
        public int TotalTickets => TicketRegistration.Sum(ticket => ticket.Value);

        
        public Dictionary<string, Dictionary<LocalDateTime, int>> EventTimeRegistration { get; set; } = new Dictionary<string, Dictionary<LocalDateTime, int>>();
        public int TotalEvents => EventTimeRegistration.Sum(events => events.Value.Sum(y => y.Value));
    }
}

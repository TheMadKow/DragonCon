using System.Collections.Generic;
using System.Linq;
using DragonCon.Modeling.Helpers;
using NodaTime;

namespace DragonCon.Features.Management.Dashboard
{
    public class ConventionStatisticsViewModel
    {
        #region Payments
        public Dictionary<string, int> PaymentCompleted { get; set; } = new Dictionary<string, int>();
        
        public int PaymentPendingCount = 1;

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
        public void AddEvent(string major, LocalDateTime localStart)
        {
            if (EventTimeRegistration == null)
                EventTimeRegistration = new Dictionary<string, Dictionary<LocalDateTime, int>>();

            if (EventTimeRegistration.MissingKey(major))
                EventTimeRegistration.Add(major, new Dictionary<LocalDateTime, int>());

            if (EventTimeRegistration[major].MissingKey(localStart))
                EventTimeRegistration[major][localStart] = 0;

            EventTimeRegistration[major][localStart]++;
        }

        #endregion

        public Modeling.Models.Conventions.Convention SelectedConvention { get; set; }
        public Dictionary<string, string> AllConventions { get; set; } = new Dictionary<string, string>();


        
        public Dictionary<string, Dictionary<LocalDateTime, int>> EventTimeRegistration { get; set; } = new Dictionary<string, Dictionary<LocalDateTime, int>>();
        public int TotalEvents => EventTimeRegistration.Sum(events => events.Value.Sum(y => y.Value));
        
        public int TotalLongTermParticipants { get; set; }
        public int TotalShortTermParticipants { get; set; }
        public int TotalParticipants => TotalLongTermParticipants + TotalShortTermParticipants;
    }
}

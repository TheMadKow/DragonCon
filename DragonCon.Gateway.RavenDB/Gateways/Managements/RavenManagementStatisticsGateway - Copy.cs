using System;
using System.Linq;
using DragonCon.Features.Management;
using DragonCon.Features.Management.Dashboard;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.Events;
using Raven.Client.Documents;

namespace DragonCon.RavenDB.Gateways.Managements
{
    public class RavenManagementStatisticsGateway : RavenGateway, IManagementStatisticsGateway
    {
        public RavenManagementStatisticsGateway(IServiceProvider provider)
            : base(provider)  {  }

        public ConventionStatisticsViewModel BuildStatisticView(string conventionId)
        {
            var viewModel = new ConventionStatisticsViewModel();

            var selectConvention = Session
                .Include<Convention>(x => x.DayIds)
                .Load<Convention>(conventionId);

            var engagements = Session.Query<UserEngagement>()
                .Include(x => x.ParticipantId)
                .Where(x => x.ConventionId == conventionId)
                .ToList();

            var activities = Session.Query<Activity>().ToDictionary(x => x.Id, x => x);
            var days = Session.Load<Day>(selectConvention.DayIds);

            viewModel.SelectedConvention = selectConvention;
            viewModel.TotalLongTermParticipants = engagements.Count(x => x.IsLongTerm);
            viewModel.TotalShortTermParticipants = engagements.Count(x => x.IsLongTerm == false);

            var actualEvents = Session
                .Query<Event>()
                .Where(x => x.ConventionId == conventionId)
                .ToList();

            foreach (var actualEvent in actualEvents)
            {
                var activity = "לא ידוע";
                if (actualEvent.ActivityId != null &&
                    activities.ContainsKey(actualEvent.ActivityId))
                {
                    activity = activities[actualEvent.ActivityId].Name;
                }

                viewModel.AddEventSeats(days, actualEvent, activity);
            }

            foreach (var engagement in engagements)
            {
                var ticketName = engagement.Payment.TicketCopy != null
                    ? engagement.Payment.TicketCopy.Name
                    : "לא שולם";
                viewModel.AddPayment(ticketName, engagement.Payment.IsPaid);

                foreach (var eventId in engagement.EventIds)
                {
                    viewModel.AddEventTakenSeat(eventId);
                }
            }

            return viewModel;
        }
    }
}

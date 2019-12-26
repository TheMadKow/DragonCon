using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DragonCon.Features.Management;
using DragonCon.Features.Management.Dashboard;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.Events;
using DragonCon.Modeling.Models.Tickets;
using NodaTime;
using Raven.Client.Documents;

namespace DragonCon.RavenDB.Gateways.Management
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

            var engagements = Session.Query<ConventionEngagement>()
                .Include(x => x.ParticipantId)
                .Include(x => x.EventIds)
                .Where(x => x.ConventionId == conventionId);

            var activities = Session.Query<Activity>().ToDictionary(x => x.Id, x => x);
            var days = Session.Load<Day>(selectConvention.DayIds);

            viewModel.SelectedConvention = selectConvention;
            viewModel.TotalLongTermParticipants = engagements.Count(x => x.IsLongTerm);
            viewModel.TotalShortTermParticipants = engagements.Count(x => x.IsLongTerm == false);
            
            foreach (var engagement in engagements)
            {
                var ticketName = engagement.Payment.TicketCopy != null
                    ? engagement.Payment.TicketCopy.Name
                    : "לא שולם";
                viewModel.AddPayment(ticketName, engagement.Payment.IsPaid);
                foreach (var eventId in engagement.EventIds)
                {
                    var evnt = Session.Load<Event>(eventId);
                    var activity = activities.ContainsKey(evnt.ActivityId)
                        ? activities[evnt.ActivityId].Name
                        : "לא ידוע";
                    var localDateTime = new LocalDateTime(
                        days[evnt.ConventionDayId].Date.Year,
                        days[evnt.ConventionDayId].Date.Month,
                        days[evnt.ConventionDayId].Date.Day,
                        evnt.TimeSlot.From.Hour,
                        evnt.TimeSlot.From.Minute);
                    
                    viewModel.AddEvent(activity, localDateTime);
                }
            }

            return viewModel;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DragonCon.Features.Management;
using DragonCon.Features.Management.Dashboard;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.Events;
using DragonCon.Modeling.Models.Tickets;
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
            viewModel.AllConventions = Session.Query<Convention>()
                .OrderByDescending(x => x.CreateTimeStamp)
                .ToDictionary(x => x.Name, x => x.Id);

            viewModel.SelectedConvention = Session
                .Include<Convention>(x => x.EventIds)
                .Include<Convention>(x => x.TicketIds)
                .Load<Convention>(conventionId);
            
            var events = Session.Load<Event>(viewModel.SelectedConvention.EventIds);
            var tickets = Session.Load<Ticket>(viewModel.SelectedConvention.TicketIds);

            // TODO consider Ticket and Event Registration
            return viewModel;
        }

       
    }
}

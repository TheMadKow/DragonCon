using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using DragonCon.Features.Management.Dashboard;
using DragonCon.Features.Management.Events;
using DragonCon.Features.Shared;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.Events;
using DragonCon.Modeling.Models.HallsTables;
using DragonCon.Modeling.Models.Identities;
using DragonCon.RavenDB.Identity;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;

namespace DragonCon.RavenDB.Gateways.Management
{
    public class RavenEventsGateway : RavenGateway, IEventsGateway
    {
        public RavenEventsGateway() : base()
        {

        }

        public RavenEventsGateway(StoreHolder holder) : base(holder)
        {
        }

        public EventsManagementViewModel BuildIndex(IDisplayPagination pagination,
            EventsManagementViewModel.Filters filters = null)
        {
            var result = new EventsManagementViewModel();
            var config = LoadSystemConfiguration();
            using (var session = OpenSession)
            {
                result.ActiveConvention = session.Load<Convention>(config.ActiveConventionId).Name;
                var tempEvents = session.Query<ConEvent>()
                    .Include(x => x.ConventionDayId)
                    .Include(x => x.GameMasterId)
                    .Include(x => x.HelperIds)
                    .Include(x => x.ActivityId)
                    .Include(x => x.SystemId)
                    .Include(x => x.HallId)
                    .Where(x => x.ConventionId == config.ActiveConventionId)
                    .OrderBy(x => x.Name)
                    .Skip(pagination.SkipCount)
                    .Take(pagination.ResultsPerPage)
                    .ToList();
                result.Events = tempEvents.Select(x => new ConEventWrapper(x)
                {
                    Day = session.Load<Day>(x.ConventionDayId),
                    EventActivity = session.Load<EventActivity>(x.ActivityId),
                    EventSystem = session.Load<EventSystem>(x.SystemId),
                    GameMaster = session.Load<RavenSystemUser>(x.GameMasterId),
                    Helpers = session.Load<RavenSystemUser>(x.HelperIds).Select(y => y.Value).ToList<IParticipant>(),
                    Hall = session.Load<Hall>(x.HallId),
                }).ToList();
            }

            return result;
        }
    }
}

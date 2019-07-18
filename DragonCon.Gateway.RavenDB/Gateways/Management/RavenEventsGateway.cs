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
using Raven.Client.Documents.Session;

namespace DragonCon.RavenDB.Gateways.Management
{
    public class RavenManagementEventsGateway : RavenGateway, IManagementEventsGateway
    {
        public RavenManagementEventsGateway() : base()
        {

        }

        public RavenManagementEventsGateway(StoreHolder holder) : base(holder)
        {
        }

        public Answer AddNewActivity(string name, List<string> systems)
        {
            using (var session = OpenSession)
            {
                var existing = session.Query<EventActivity>().Where(x => x.Name == name).ToList();
                if (existing.Any())
                {
                    return Answer.Error("קיימת פעילות בשם שהוזן");
                }

                var activity = new EventActivity();
                activity.Name = name;
                activity.ActivitySystems = new List<EventSystem>();
                foreach (var system in systems)
                {
                    var eventSystem = new EventSystem
                    {
                        Name = system
                    };
                    session.Store(eventSystem);
                    activity.ActivitySystems.Add(eventSystem);
                }

                session.Store(activity);
                session.SaveChanges();
                return Answer.Success;
            }
        }

        public EventsManagementViewModel BuildIndex(IDisplayPagination pagination,
            EventsManagementViewModel.Filters filters = null)
        {
            var result = new EventsManagementViewModel();
            var config = LoadSystemConfiguration();
            using (var session = OpenSession)
            {
                result.ActiveConvention = session.Load<Convention>(config.ActiveConventionId).Name;
                result.Activities = session.Query<EventActivity>().Include(x => x.ActivitySystems).ToList();
                result.AgeRestrictions = session.Query<AgeRestriction>().ToList();
                
                var tempEvents = session.Query<ConEvent>()
                    .Statistics(out var stats)
                    .Include(x => x.ConventionDayId)
                    .Include(x => x.GameMasterId)
                    .Include(x => x.HelperIds)
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
                result.Pagination = DisplayPagination.BuildForView(
                    stats.TotalResults, 
                    pagination.SkipCount,
                    pagination.ResultsPerPage);
            }
            return result;
        }
    }
}

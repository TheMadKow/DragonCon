using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using DragonCon.Features.Management.Dashboard;
using DragonCon.Features.Management.Events;
using DragonCon.Features.Shared;
using DragonCon.Modeling.Helpers;
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

        public Answer UpdateExistingActivity(string activityId, string activityName, List<SystemViewModel> filteredList)
        {
            using (var session = OpenSession)
            {
                var activity = session.Load<EventActivity>(activityId);
                if (activity == null)
                    throw new Exception("Unknown activity ID");

                var existingSystems = filteredList
                    .Where(x => x.Id.IsNotEmptyString())
                    .ToDictionary(x => x.Id, x => x.Name);
                var newSystems = filteredList.Where(x => x.Id.IsEmptyString());
                var removedSystems = new List<EventSystem>();

                foreach (var activitySystem in activity.ActivitySystems)
                {
                    if (existingSystems.MissingKey(activitySystem.Id))
                    {
                        removedSystems.Add(activitySystem);;
                        session.Delete(activitySystem.Id);
                    }
                    else
                    {
                        activitySystem.Name = existingSystems[activitySystem.Id];
                        session.Store(activitySystem);
                    }
                }

                foreach (var removedSystem in removedSystems)
                {
                    activity.ActivitySystems.Remove(removedSystem);
                }

                foreach (var newSystem in newSystems)
                {
                    var eventSystem = new EventSystem
                    {
                        Name = newSystem.Name
                    };
                    session.Store(eventSystem);
                    activity.ActivitySystems.Add(eventSystem);
                }

                activity.Name = activityName;

                session.SaveChanges();
                return Answer.Success;
            }
        }

        public Answer DeleteActivity(string activityId)
        {
            using (var session = OpenSession)
            {
                var activity = session.Load<EventActivity>(activityId);
                if (activity == null)
                    throw new Exception("Unknown activity ID");

                foreach (var system in activity.ActivitySystems)
                {
                    session.Delete(system.Id);
                }

                session.Delete(activity.Id);
                session.SaveChanges();
            }

            return Answer.Success;
        }

        public ActivitySystemCreateUpdateViewModel GetActivityViewModel(string activityId)
        {
            using (var session = OpenSession)
            {
                activityId = activityId.FixRavenId("EventActivities");
                var activity = session.Load<EventActivity>(activityId);
                if (activity == null)
                    throw new Exception("Unknown activity ID");

                return new ActivitySystemCreateUpdateViewModel()
                {
                    Name = activity.Name,
                    Id = activity.Id,
                    Systems = activity.ActivitySystems.Select(x => new SystemViewModel(x)).ToList()
                };
            }
        }

        public Answer AddOrUpdateAgeRestriction(AgeSystemCreateUpdateViewModel viewmodel)
        {
            using (var session = OpenSession)
            {
                var ageRestriction = new AgeRestriction();
                if (viewmodel.Id.IsNotEmptyString())
                {
                    ageRestriction = session.Load<AgeRestriction>(viewmodel.Id);
                    if (ageRestriction == null)
                        throw new Exception("Unknown Age Restriction");
                }

                ageRestriction.Name = viewmodel.Name;
                ageRestriction.MaxAge = viewmodel.MaxAge;
                ageRestriction.MinAge = viewmodel.MinAge;

                session.Store(ageRestriction);
                session.SaveChanges();

                return Answer.Success;
            }
        }

        public AgeSystemCreateUpdateViewModel GetAgeRestrictionViewModel(string restrictionId)
        {
            using (var session = OpenSession)
            {
                restrictionId = restrictionId.FixRavenId("AgeRestrictions");
                var ageRestriction = session.Load<AgeRestriction>(restrictionId);
                if (ageRestriction == null)
                    throw new Exception("Unknown Age Restriction");

                return new AgeSystemCreateUpdateViewModel()
                {
                    Id = ageRestriction.Id,
                    Name = ageRestriction.Name,
                    MaxAge = ageRestriction.MaxAge,
                    MinAge = ageRestriction.MinAge
                };
            }
        }

        public Answer DeleteAgeRestriction(string restrictionId)
        {
            using (var session = OpenSession)
            {
                var ageRestriction = session.Load<AgeRestriction>(restrictionId);
                if (ageRestriction == null)
                    return Answer.Error("Unknown Age Restriction");

                session.Delete(ageRestriction);
                session.SaveChanges();
                return Answer.Success;
            }
        }

        public EventCreateUpdateViewModel GetEventViewModel(string eventId)
        {
            using (var session = OpenSession)
            {
                var config = LoadSystemConfiguration(session);
                var currentConvention = session
                    .Include<Convention>(x => x.DayIds)
                    .Include<Convention>(x => x.HallIds)
                    .Load<Convention>(config.ActiveConventionId);

                var viewModel = new EventCreateUpdateViewModel
                {
                    Activities = session.Query<EventActivity>().Include(x => x.ActivitySystems).ToList(),
                    Days = session.Load<Day>(currentConvention.DayIds).Select(x => x.Value),
                    Halls = session.Load<Hall>(currentConvention.HallIds).Select(x => x.Value),
                    AgeRestrictions = session.Query<AgeRestriction>().ToList()
                };

                if (eventId.IsNotEmptyString())
                {
                    // TODO
                }
                else
                {
                    if (viewModel.HelperIds == null)
                        viewModel.HelperIds = new List<string>();
                    if (viewModel.Size == null)
                        viewModel.Size = new SizeRestriction();
                    if (viewModel.Tags == null)
                        viewModel.Tags = new List<string>();
                    if (viewModel.TimeSlot == null)
                        viewModel.TimeSlot = new TimeSlot();
                    viewModel.Status = EventStatus.Pending;
                }

                return viewModel;
            }

        }

        public EventsManagementViewModel BuildIndex(IDisplayPagination pagination,
            EventsManagementViewModel.Filters filters = null)
        {
            var result = new EventsManagementViewModel();
            using (var session = OpenSession)
            {
                var config = LoadSystemConfiguration(session);
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

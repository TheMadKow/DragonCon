using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
using NodaTime;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using SystemClock = Microsoft.Extensions.Internal.SystemClock;

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
                    eventId = eventId.FixRavenId("ConEvents");
                    var existing = session.Load<ConEvent>(eventId);
                    viewModel.Duration = (int) existing.TimeSlot.Span.Hours;
                    viewModel.StartTime = new DateTime(1, 1,1, existing.TimeSlot.From.Hour, existing.TimeSlot.From.Minute, existing.TimeSlot.From.Second);
                    viewModel.Name = existing.Name;
                    viewModel.GameMasterIds = existing.GameMasterIds;
                    viewModel.SpecialRequests = existing.SpecialRequests;
                    viewModel.Status = existing.Status;
                    viewModel.Tags = existing.Tags;
                    viewModel.AgeRestrictionId = existing.AgeId;
                    viewModel.Size = existing.Size;
                    viewModel.ConventionDayId = existing.ConventionDayId;
                    viewModel.Description = existing.Description;
                    viewModel.Id = existing.Id;
                    viewModel.IsSpecialPrice = existing.IsSpecialPrice;
                    viewModel.SpecialPrice = existing.SpecialPrice;
                    viewModel.Table = $"{existing.HallId},{existing.Table}";
                    viewModel.SystemId = $"{existing.ActivityId},{existing.SystemId}";
                }
                else
                {
                    if (viewModel.GameMasterIds == null)
                        viewModel.GameMasterIds = new List<string>();
                    if (viewModel.Size == null)
                        viewModel.Size = new SizeRestriction();
                    if (viewModel.Tags == null)
                        viewModel.Tags = new List<string>();
                    viewModel.Status = EventStatus.Pending;
                }

                return viewModel;
            }

        }

        public Answer UpdateEvent(EventCreateUpdateViewModel viewmodel)
        {
            var answer = ValidateEventFields(viewmodel);
            if (answer.AnswerType != AnswerType.Success)
                return answer;

            using (var session = OpenSession)
            {
                var model = session
                    .Include<ConEvent>(x => x.UserActionsId)
                    .Load<ConEvent>(viewmodel.Id);

                if (model == null)
                    return Answer.Error("תקלה בטעינת האירוע");

                var userActionList = session.Load<UserActionList>(model.UserActionsId);
                var changes = GetUserActions(model, viewmodel);
                userActionList.Actions.AddRange(changes);

                model.UpdatedOn = NodaTime.SystemClock.Instance.GetCurrentInstant();
                model.IsSpecialPrice = viewmodel.IsSpecialPrice;
                model.SpecialPrice = viewmodel.SpecialPrice;
                model.Name = viewmodel.Name;
                model.Description = viewmodel.Description;
                model.ConventionDayId = viewmodel.ConventionDayId;
                model.Status = viewmodel.Status;
                model.GameMasterIds = viewmodel.GameMasterIds;
                model.Size = viewmodel.Size;
                model.AgeId = viewmodel.AgeRestrictionId;
                model.Tags = viewmodel.Tags;
                model.SpecialRequests = viewmodel.SpecialRequests;
                
                var activity = viewmodel.SystemId.Split(new[] {','}, StringSplitOptions.None);
                model.ActivityId = activity[0];
                model.SystemId = activity[1];
                
                var hall = viewmodel.Table.Split(new[] {','}, StringSplitOptions.None);
                model.HallId = hall[0];
                model.Table = int.Parse(hall[1]);

                var startTime = new LocalTime(viewmodel.StartTime.Value.Hour,
                    viewmodel.StartTime.Value.Minute,
                    viewmodel.StartTime.Value.Second);
                var endTime = startTime.PlusHours(viewmodel.Duration.Value);
                model.TimeSlot = new TimeSlot
                {
                    From = startTime,
                    To = endTime
                };

                session.SaveChanges();
            }
            return Answer.Success;
        }

        private List<UserAction> GetUserActions(ConEvent model, EventCreateUpdateViewModel viewmodel)
        {
            var result = new List<UserAction>();
            foreach (PropertyInfo property in model.GetType().GetProperties())
            {
                var value1 = property.GetValue(model, null);
                var compareProperty = viewmodel.GetType().GetProperty(property.Name);
                if (compareProperty != null)
                {
                    var value2 = compareProperty.GetValue(viewmodel, null);
                    if (!value1.Equals(value2))
                    {
                        result.Add(new UserAction()
                        {
                            UserId = "",
                            Field = property.Name,
                            OldValue = value1.ToString(),
                            NewValue = value2.ToString(),
                            TimeStamp = NodaTime.SystemClock.Instance.GetCurrentInstant()
                        });
                    }
                }
            }

            return result;
        }


        public Answer CreateEvent(EventCreateUpdateViewModel viewmodel)
        {
            var answer = ValidateEventFields(viewmodel);
            if (answer.AnswerType != AnswerType.Success)
                return answer;

            using (var session = OpenSession)
            {
                var model = new ConEvent();
                var userActionList = new UserActionList()
                {
                    Actions = new List<UserAction>()
                    {
                        new UserAction()
                        {
                            Field = "Event Creation",
                            TimeStamp = NodaTime.SystemClock.Instance.GetCurrentInstant(),
                            UserId = ""
                        }
                    }
                };

                session.Store(userActionList);
                model.CreatedOn = NodaTime.SystemClock.Instance.GetCurrentInstant();
                model.ConventionId = LoadSystemConfiguration(session).ActiveConventionId;
                model.UserActionsId = userActionList.Id;
                model.IsSpecialPrice = viewmodel.IsSpecialPrice;
                model.SpecialPrice = viewmodel.SpecialPrice;
                model.Name = viewmodel.Name;
                model.Description = viewmodel.Description;
                model.ConventionDayId = viewmodel.ConventionDayId;
                model.Status = viewmodel.Status;
                model.GameMasterIds = viewmodel.GameMasterIds;
                model.Size = viewmodel.Size;
                model.AgeId = viewmodel.AgeRestrictionId;
                model.Tags = viewmodel.Tags;
                model.SpecialRequests = viewmodel.SpecialRequests;

                if (viewmodel.SystemId.IsNotEmptyString())
                {
                    var activity = viewmodel.SystemId.Split(new[] {','}, StringSplitOptions.None);
                    model.ActivityId = activity[0];
                    model.SystemId = activity[1];
                }

                if (viewmodel.Table.IsNotEmptyString())
                {
                    var hall = viewmodel.Table.Split(new[] {','}, StringSplitOptions.None);
                    model.HallId = hall[0];
                    model.Table = int.Parse(hall[1]);
                }

                if (viewmodel.StartTime.HasValue && viewmodel.Duration.HasValue)
                {

                    var startTime = new LocalTime(viewmodel.StartTime.Value.Hour,
                        viewmodel.StartTime.Value.Minute,
                        viewmodel.StartTime.Value.Second);
                    var endTime = startTime.PlusHours(viewmodel.Duration.Value);
                    model.TimeSlot = new TimeSlot
                    {
                        From = startTime,
                        To = endTime
                    };
                }

                session.Store(model);
                session.SaveChanges();
            }
            return Answer.Success;
        }

        
        private static Answer ValidateEventFields(EventCreateUpdateViewModel viewmodel)
        {
            if (viewmodel.IsSpecialPrice && viewmodel.SpecialPrice == null)
                return Answer.Error("אי אפשר הגדיר מחיר מיוחד ריק");
            if (viewmodel.Name.IsEmptyString())
                return Answer.Error("אי אפשר להגדיר אירוע ללא שם");
            if (viewmodel.Status == EventStatus.Approved)
            {
                if (viewmodel.SystemId.IsEmptyString())
                    return Answer.Error("אי אפשר לאשר אירוע ללא סוג פעילות");
                if (viewmodel.AgeRestrictionId.IsEmptyString())
                    return Answer.Error("אי אפשר לאשר אירוע ללא קבוצת גיל");
                if (viewmodel.ConventionDayId.IsEmptyString())
                    return Answer.Error("אי אפשר לאשר אירוע ללא יום");
                if (viewmodel.StartTime.HasValue == false || viewmodel.Duration.HasValue == false || viewmodel.Duration <= 0)
                    return Answer.Error("אי אפשר לאשר אירוע ללא מסגרת זמן");
                if (viewmodel.GameMasterIds.Any() == false)
                    return Answer.Error("אי אפשר לאשר אירוע ללא הנחיה");
            }

            return Answer.Success;
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
                    .Include(x => x.GameMasterIds)
                    .Include(x => x.HallId)
                    .Include(x => x.AgeId)
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
                    GameMasters = session.Load<FullParticipant>(x.GameMasterIds).Select(y => y.Value).ToList<IParticipant>(),
                    Hall = session.Load<Hall>(x.HallId),
                    AgeRestriction = session.Load<AgeRestriction>(x.AgeId)
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

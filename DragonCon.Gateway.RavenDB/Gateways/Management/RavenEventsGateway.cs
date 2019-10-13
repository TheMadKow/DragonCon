using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using DragonCon.Features.Management.Dashboard;
using DragonCon.Features.Management.Events;
using DragonCon.Features.Shared;
using DragonCon.Modeling.Helpers;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.Events;
using DragonCon.Modeling.Models.HallsTables;
using DragonCon.Modeling.Models.Identities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NodaTime;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using StackExchange.Profiling.Internal;
using Activity = DragonCon.Modeling.Models.Common.Activity;

namespace DragonCon.RavenDB.Gateways.Management
{
    public class RavenManagementEventsGateway : RavenGateway, IManagementEventsGateway
    {
        public RavenManagementEventsGateway(StoreHolder holder, IActor actor) :
            base(holder, actor)
        {
        }

        public Answer AddNewActivity(string name, List<string> subActivities)
        {
            var existing = Session.Query<Activity>().Where(x => x.Name == name && x.IsSubActivity == false).ToList();
            if (existing.Any())
            {
                return Answer.Error("קיימת פעילות בשם שהוזן");
            }

            var activity = new Activity();
            activity.Name = name;
            activity.SubActivities = new List<Activity>();
            foreach (var subActivity in subActivities)
            {
                var eventActivity = new Activity
                {
                    Name = subActivity,
                    IsSubActivity = true
                };
                Session.Store(eventActivity);
                activity.SubActivities.Add(eventActivity);
            }

            Session.Store(activity);
            Session.SaveChanges();
            return Answer.Success;
        }

        public Answer UpdateExistingActivity(string activityId, string activityName,
            List<SubActivityViewModel> filteredList)
        {
            var activity = Session.Load<Activity>(activityId);
            if (activity == null)
                throw new Exception("Unknown activity ID");

            var existing = Session.Query<Activity>()
                .Where(x => x.Name == activityName && x.IsSubActivity == false && x.IsSubActivity == false).ToList();
            if (existing.Any())
            {
                return Answer.Error("קיימת פעילות בשם שהוזן");
            }

            var exisitingSubActivities = filteredList
                .Where(x => x.Id.IsNotEmptyString())
                .ToDictionary(x => x.Id, x => x.Name);
            var newSubActivities = filteredList.Where(x => x.Id.IsEmptyString());
            var removedSubActivities = new List<Activity>();

            foreach (var subActivity in activity.SubActivities)
            {
                if (exisitingSubActivities.MissingKey(subActivity.Id))
                {
                    removedSubActivities.Add(subActivity);
                    ;
                    Session.Delete(subActivity.Id);
                }
                else
                {
                    subActivity.Name = exisitingSubActivities[subActivity.Id];
                    Session.Store(subActivity);
                }
            }

            foreach (var removedSystem in removedSubActivities)
            {
                activity.SubActivities.Remove(removedSystem);
            }

            foreach (var newSubActivity in newSubActivities)
            {
                var subActivity = new Activity
                {
                    Name = newSubActivity.Name,
                    IsSubActivity = true
                };
                Session.Store(subActivity);
                activity.SubActivities.Add(subActivity);
            }

            activity.Name = activityName;

            Session.SaveChanges();
            return Answer.Success;
        }

        public Answer DeleteActivity(string activityId)
        {
            var activity = Session.Load<Activity>(activityId);
            if (activity == null)
                throw new Exception("Unknown activity ID");

            foreach (var system in activity.SubActivities)
            {
                Session.Delete(system.Id);
            }

            Session.Delete(activity.Id);
            Session.SaveChanges();

            return Answer.Success;
        }

        public ActivityCreateUpdateViewModel GetActivityViewModel(string activityId)
        {
            activityId = activityId.FixRavenId("Activities");
            var activity = Session.Load<Activity>(activityId);
            if (activity == null)
                throw new Exception("Unknown activity ID");

            return new ActivityCreateUpdateViewModel()
            {
                Name = activity.Name,
                Id = activity.Id,
                SubActivities = activity.SubActivities.Select(x => new SubActivityViewModel(x)).ToList()
            };
        }

        public Answer AddOrUpdateAgeRestriction(AgeSystemCreateUpdateViewModel viewmodel)
        {
            var ageRestriction = new AgeGroup();
            if (viewmodel.Id.IsNotEmptyString())
            {
                ageRestriction = Session.Load<AgeGroup>(viewmodel.Id);
                if (ageRestriction == null)
                    throw new Exception("Unknown Age Restriction");
            }

            ageRestriction.Name = viewmodel.Name;
            ageRestriction.MaxAge = viewmodel.MaxAge;
            ageRestriction.MinAge = viewmodel.MinAge;

            Session.Store(ageRestriction);
            Session.SaveChanges();

            return Answer.Success;

        }

        public AgeSystemCreateUpdateViewModel GetAgeRestrictionViewModel(string restrictionId)
        {
            restrictionId = restrictionId.FixRavenId("AgeGroups");
            var ageRestriction = Session.Load<AgeGroup>(restrictionId);
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

        public Answer DeleteAgeRestriction(string restrictionId)
        {
            var ageRestriction = Session.Load<AgeGroup>(restrictionId);
            if (ageRestriction == null)
                return Answer.Error("Unknown Age Restriction");

            Session.Delete(ageRestriction);
            Session.SaveChanges();
            return Answer.Success;
        }

        public EventCreateUpdateViewModel GetEventViewModel(string eventId)
        {
            var viewModel = new EventCreateUpdateViewModel();

            if (eventId.IsNotEmptyString())
            {
                eventId = eventId.FixRavenId("Events");
                var existing = Session.Load<Event>(eventId);
                viewModel.Duration = existing.TimeSlot?.Duration;
                viewModel.Name = existing.Name;
                viewModel.GameMasterIds = existing.GameMasterIds;
                viewModel.SpecialRequests = existing.SpecialRequests;
                viewModel.Status = existing.Status;
                viewModel.Tags = existing.Tags;
                viewModel.AgeId = existing.AgeId;
                viewModel.Size = existing.Size;
                viewModel.Description = existing.Description;
                viewModel.Id = existing.Id;
                viewModel.IsSpecialPrice = existing.IsSpecialPrice;
                viewModel.SpecialPrice = existing.SpecialPrice;
                viewModel.HallTableSelector = $"{existing.HallId},{existing.HallTable}";
                viewModel.ActivitySelector = $"{existing.ActivityId},{existing.SubActivityId}";
                viewModel.StartTimeSelector = $"{existing.ConventionDayId},{existing.TimeSlot?.From.ToString("HH:mm", CultureInfo.CurrentCulture)}";
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

        public Answer UpdateEvent(EventCreateUpdateViewModel viewmodel)
        {
            var answer = ValidateEventFields(viewmodel);
            if (answer.AnswerType != AnswerType.Success)
                return answer;

            var model = Session.Load<Event>(viewmodel.Id);

            if (model == null)
                return Answer.Error("תקלה בטעינת האירוע");

            var userActions = GetUserActions(model, viewmodel);
            foreach (var userAction in userActions.list)
            {
                Session.Store(userAction);
            }

            model.UpdatedOn = SystemClock.Instance.GetCurrentInstant();
            model.IsSpecialPrice = viewmodel.IsSpecialPrice;
            model.SpecialPrice = viewmodel.SpecialPrice;
            model.Name = viewmodel.Name;
            model.Description = viewmodel.Description;
            model.ConventionDayId = viewmodel.ConventionDayId;
            model.Status = viewmodel.Status;
            model.GameMasterIds = viewmodel.GameMasterIds;
            model.Size = viewmodel.Size;
            model.AgeId = viewmodel.AgeId;
            model.Tags = viewmodel.Tags;
            model.SpecialRequests = viewmodel.SpecialRequests;
            model.ActivityId = viewmodel.ActivityId;
            model.SubActivityId = viewmodel.SubActivityId;
            model.HallId = viewmodel.HallId;
            model.HallTable = int.Parse(viewmodel.HallTable);
            
            if (viewmodel.StartTimeSelector.IsNotEmptyString() && viewmodel.Duration.HasValue)
            {
                var timeSplit = viewmodel.StartTime.Split(':');
                var hours = int.Parse(timeSplit[0]);
                var minutes = int.Parse(timeSplit[1]);
                var num = (int) viewmodel.Duration.Value / 1;
                var denum = (int) viewmodel.Duration.Value % 1;
                
                var startTime = new LocalTime(hours, minutes);
                var endTime = startTime.PlusHours(num).PlusMinutes(denum);
                
                var tempTimeSlot = new TimeSlot
                {
                    From = startTime,
                    To = endTime,
                    Duration = viewmodel.Duration.Value
                };

                if (model.TimeSlot.AsJson() != tempTimeSlot.AsJson())
                {
                    var action = new UserAction()
                    {
                        TimeStamp = userActions.timestamp,
                        Field = "TimeSlot",
                        DocumentId = model.Id,
                        NewValue = tempTimeSlot.AsJson(),
                        OldValue = model.TimeSlot.AsJson(),
                        UserId = Actor.Participant.Id
                    };
                    model.TimeSlot = tempTimeSlot;
                    Session.Store(action);
                }
            }

            Session.SaveChanges();
            return Answer.Success;
        }

        private (Instant timestamp, List<UserAction> list) GetUserActions(Event model, EventCreateUpdateViewModel viewmodel)
        {
            var timestamp = SystemClock.Instance.GetCurrentInstant();
            var result = new List<UserAction>();
            foreach (PropertyInfo property in model.GetType().GetProperties())
            {
                var value1 = property.GetValue(model, null);
                var compareProperty = viewmodel.GetType().GetProperty(property.Name);
                if (compareProperty != null)
                {
                    var value2 = compareProperty.GetValue(viewmodel, null);

                    var value1AsJson = value1.AsJson();
                    var value2AsJson = value2.AsJson();

                    if ((value1 is null && value2 is {}) ||
                        (value1 is {} && value2 is null) ||
                        (value1 is {} && value2 is {} && value1AsJson != value2AsJson))
                    {

                        result.Add(new UserAction
                        {
                            DocumentId = model.Id,
                            UserId = Actor.Participant.Id,
                            Field = property.Name,
                            OldValue = value1AsJson ?? string.Empty,
                            NewValue = value2AsJson ?? string.Empty,
                            TimeStamp = timestamp
                        });
                    }
                }
            }

            return (timestamp, result);
        }


        public Answer CreateEvent(EventCreateUpdateViewModel viewmodel)
        {
            var answer = ValidateEventFields(viewmodel);
            if (answer.AnswerType != AnswerType.Success)
                return answer;

            var model = new Event
            {
                CreatedOn = SystemClock.Instance.GetCurrentInstant(),
                ConventionId = Actor.SystemState.ConventionId,
                IsSpecialPrice = viewmodel.IsSpecialPrice,
                SpecialPrice = viewmodel.SpecialPrice,
                Name = viewmodel.Name,
                Description = viewmodel.Description,
                ConventionDayId = viewmodel.ConventionDayId,
                Status = viewmodel.Status,
                GameMasterIds = viewmodel.GameMasterIds,
                Size = viewmodel.Size,
                AgeId = viewmodel.AgeId,
                Tags = viewmodel.Tags,
                SpecialRequests = viewmodel.SpecialRequests
            };

            if (viewmodel.ActivitySelector.IsNotEmptyString())
            {
                var activity = viewmodel.ActivitySelector.Split(new[] {','}, StringSplitOptions.None);
                model.ActivityId = activity[0];
                model.SubActivityId = activity[1];
            }

            if (viewmodel.HallTableSelector.IsNotEmptyString())
            {
                var hall = viewmodel.HallTableSelector.Split(new[] {','}, StringSplitOptions.None);
                model.HallId = hall[0];
                model.HallTable = int.Parse(hall[1]);
            }

            if (viewmodel.StartTimeSelector.IsNotEmptyString() && viewmodel.Duration.HasValue)
            {
                var split = viewmodel.StartTimeSelector.Split(',');
                var day = split[0];
                var time = split[1];
                var timeSplit = time.Split(':');
                var hours = int.Parse(timeSplit[0]);
                var minutes = int.Parse(timeSplit[1]);
                var num = (int) viewmodel.Duration.Value / 1;
                var denum = (int) viewmodel.Duration.Value % 1;
                
                model.ConventionDayId = day;
                var startTime = new LocalTime(hours, minutes);
                var endTime = startTime.PlusHours(num).PlusMinutes(denum);
                
                model.TimeSlot = new TimeSlot
                {
                    From = startTime,
                    To = endTime,
                    Duration = viewmodel.Duration.Value
                };
            }

            Session.Store(model);
            var userAction = new UserAction
            {
                UserId = Actor.Participant.Id,
                DocumentId = model.Id,

                Field = "Event Creation",
                TimeStamp = SystemClock.Instance.GetCurrentInstant(),
            };

            Session.Store(userAction);
            Session.SaveChanges();

            return Answer.Success;
        }


        private static Answer ValidateEventFields(EventCreateUpdateViewModel viewmodel)
        {
            if (viewmodel.IsSpecialPrice && viewmodel.SpecialPrice == null)
                return Answer.Error("אי אפשר להגדיר מחיר מיוחד ריק");
            if (viewmodel.Name.IsEmptyString())
                return Answer.Error("אי אפשר להגדיר אירוע ללא שם");
            if (viewmodel.Status == EventStatus.Approved)
            {
                if (viewmodel.ActivitySelector.IsEmptyString())
                    return Answer.Error("אי אפשר לאשר אירוע ללא סוג פעילות");
                if (viewmodel.AgeId.IsEmptyString())
                    return Answer.Error("אי אפשר לאשר אירוע ללא קבוצת גיל");
                if (viewmodel.ConventionDayId.IsEmptyString())
                    return Answer.Error("אי אפשר לאשר אירוע ללא יום");
                if (viewmodel.StartTimeSelector.IsNotEmptyString() || viewmodel.Duration.HasValue == false ||
                    viewmodel.Duration <= 0)
                    return Answer.Error("אי אפשר לאשר אירוע ללא מסגרת זמן");
                if (viewmodel.GameMasterIds.Any() == false)
                    return Answer.Error("אי אפשר לאשר אירוע ללא הנחיה");
            }

            return Answer.Success;
        }

        public EventsManagementViewModel BuildIndex(IDisplayPagination pagination,
            EventsManagementViewModel.Filters? filters = null)
        {
            var result = new EventsManagementViewModel();

            var tempEvents = Session.Query<Event>()
                .Statistics(out var stats)
                .Include(x => x.ConventionDayId)
                .Include(x => x.GameMasterIds)
                .Include(x => x.HallId)
                .Include(x => x.AgeId)
                .Where(x => x.ConventionId == Actor.SystemState.ConventionId);

            if (filters is {})
            {
                if (filters.AgeGroup.IsNotEmptyString())
                {
                    tempEvents = tempEvents.Where(x => x.AgeId == filters.AgeGroup);
                }
                if (filters.Duration > 0)
                {
                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    tempEvents = tempEvents.Where(x => x.TimeSlot.Duration == filters.Duration);

                }
                if (filters.Activity.IsNotEmptyString())
                {
                    var activity = filters.Activity.SplitTuples();
                    tempEvents = tempEvents.Where(x => x.ActivityId == activity.Major);
                    if (activity.Minor.IsNotEmptyString())
                    {
                        tempEvents = tempEvents.Where(x => x.SubActivityId == activity.Minor);
                    }
                }

                if (filters.DayAndTime.IsNotEmptyString())
                {
                    var dayTime = filters.DayAndTime.SplitTuples();
                    tempEvents = tempEvents.Where(x => x.ConventionDayId == dayTime.Major);
                    if (dayTime.Minor.IsNotEmptyString())
                    {
                        //tempEvents.Where(x => x.TimeSlot.From == dayTime.Minor);
                    }
                }

                if (filters.Status.IsNotEmptyString())
                {
                    if (Enum.TryParse(filters.Status, true, out EventStatus status))
                    {
                        tempEvents = tempEvents.Where(x => x.Status == status);
                    }
                }
            }

            var results =
                tempEvents.OrderBy(x => x.Name)
                .Skip(pagination.SkipCount)
                .Take(pagination.ResultsPerPage)
                .ToList();

            result.Events = results.Select(x => new EventWrapper(x)
            {
                Day = Session.Load<Day>(x.ConventionDayId),
                Activity = Session.Load<Activity>(x.ActivityId),
                SubActivity = x.SubActivityId.IsNotEmptyString() 
                    ? Session.Load<Activity>(x.SubActivityId) 
                    : Activity.General,
                GameMasters =
                    Session.Load<FullParticipant>(x.GameMasterIds).Select(y => y.Value).ToList<IParticipant>(),
                Hall = Session.Load<Hall>(x.HallId),
                AgeGroup = Session.Load<AgeGroup>(x.AgeId)
            }).ToList();

            result.Pagination = DisplayPagination.BuildForView(
                stats.TotalResults,
                pagination.SkipCount,
                pagination.ResultsPerPage);

            return result;
        }

        public EventHistoryViewModel CreateEventHistory(string eventId)
        {
            var actions = Session.Query<UserAction>()
                .Where(x => x.DocumentId == eventId)
                .OrderByDescending(x => x.TimeStamp)
                .ToList();
 
            return new EventHistoryViewModel()
            {
                Actions = actions
            };
        }
    }
}
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DragonCon.Modeling.Helpers;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.Events;
using DragonCon.Modeling.Models.HallsTables;
using DragonCon.Modeling.Models.Tickets;
using DragonCon.Modeling.Models.UserDisplay;
using DragonCon.Modeling.TimeSlots;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DragonCon.Modeling.Models.Identities
{

    public interface IActor
    {
        Actor.ActorParticipant Me { get; set; }
        Actor.ActorSystemState System { get; set; }

        bool HasDisplayConvention { get; set; }
        bool HasManagedConvention { get; set; }

        // Only Manager Loaded
        Actor.ActorConventionState ManagedConvention { get; set; }
        Actor.ActorDropDowns ManagedDropDowns { get; set; }

        // Public Load
        Actor.ActorConventionState DisplayConvention { get; set; }
        Actor.ActorDropDowns DisplayDropDowns { get; set; }
        Actor.ActorDynamicContent DisplayDynamics { get; set; }

        // Roles
        bool HasAnySystemRole { get;  }
        bool HasSystemRole(SystemRoles role);

        // Meta
        long LoadTimeInMs { get; set; }
    }

    public class Actor : IActor
    {
        public Actor()
        {
            Me = new ActorParticipant();
            System = new ActorSystemState();

            ManagedConvention = new ActorConventionState();
            DisplayConvention = new ActorConventionState();

            DisplayDropDowns = new ActorDropDowns();
            ManagedDropDowns = new ActorDropDowns();

            DisplayDynamics = new ActorDynamicContent();
        }

        public ActorParticipant Me { get; set; }
        public ActorSystemState System { get; set; }

        public bool HasDisplayConvention { get; set; }
        public bool HasManagedConvention { get; set; }

        public ActorConventionState DisplayConvention { get; set; } = null;
        public ActorConventionState ManagedConvention { get; set; } = null;

        public ActorDropDowns DisplayDropDowns { get; set; } = null;
        public ActorDynamicContent DisplayDynamics { get; set; } = null;
        public ActorDropDowns ManagedDropDowns { get; set; } = null;

        public bool HasAnySystemRole => Me.SystemRoles.Any();
        public bool HasSystemRole(SystemRoles role) => Me.SystemRoles.Contains(role);
        public long LoadTimeInMs { get; set; }

        #region Subclasses
        public class ActorDropDowns
        {
            private readonly ActorConventionState _convention;
            private readonly ActorSystemState _system;
            private readonly IStrategyFactory _factory;

            public ActorDropDowns()
            {

            }

            public ActorDropDowns(IStrategyFactory factory, ActorConventionState convention, ActorSystemState system)
            {
                _convention = convention;
                _system = system;
                _factory = factory;
            }

            public List<SelectListItem> BuildHalls()
            {
                if (_convention.Halls is null)
                    return new List<SelectListItem>();

                var items = new List<SelectListItem>();
                foreach (var halls in _convention.Halls.OrderBy(x => x.FirstTable))
                {
                    var group = new SelectListGroup {Name = halls.Name};
                    foreach (var table in halls.Tables.OrderBy(x => x))
                    {
                        var item = new SelectListItem
                        {
                            Value = $"{halls.Id},{table}",
                            Text = $"שולחן {table}",
                            Group = group
                        };
                        items.Add(item);
                    }
                }

                return items;
            }

            public List<SelectListItem> BuildAgeGroups()
            {
                if (_system.AgeGroups == null)
                    return new List<SelectListItem>();

                return _system.AgeGroups
                    .OrderBy(x => x.Name)
                    .Select(x => new SelectListItem(x.GetDescription(), x.Id)).ToList();
            }

            public List<SelectListItem> Activities(bool addGeneralSelect = true)
            {
                var items = new List<SelectListItem>();
                if (_system.Activities == null)
                    return new List<SelectListItem>();

                foreach (var eventActivity in _system.Activities.OrderBy(x => x.Name))
                {
                    var group = new SelectListGroup
                    {
                        Name = eventActivity.Name
                    };
                    if (addGeneralSelect)
                    {
                        items.Add(new SelectListItem
                        {
                            Value = $"{eventActivity.Id},",
                            Text = "כללי",
                            Group = group
                        });
                    }

                    foreach (var subActivity in eventActivity.SubActivities.OrderBy(x => x.Name))
                    {
                        var item = new SelectListItem
                        {
                            Value = $"{eventActivity.Id},{subActivity.Id}",
                            Text = subActivity.Name,
                            Group = group
                        };
                        items.Add(item);
                    }
                }

                return items;
            }

            public List<SelectListItem> BuildStatus()
            {
                return Enums.AsSelectListItem<EventStatus>().ToList();
            }

            public List<SelectListItem> BuildDaysTimes()
            {
                if (_convention.Days is null)
                    return new List<SelectListItem>();

                var items = new List<SelectListItem>();
                foreach (var day in _convention.Days.OrderBy(x => x.Date))
                {
                    var group = new SelectListGroup
                    {
                        Name = day.GetDescription()
                    };
              
                    var timeSlots = _factory.GenerateTimeSlots(day.StartTime, day.EndTime, day.TimeSlotStrategy);
                    foreach (var starTime in timeSlots.StartTimeAndDurations.Keys)
                    {
                        var item = new SelectListItem
                        {
                            Value = $"{day.Id},{starTime.ToString("HH:mm", CultureInfo.CurrentCulture)}",
                            Text = $"{starTime.ToString("HH:mm", CultureInfo.CurrentCulture)}",
                            Group = group
                        };
                        items.Add(item);
                    }
                }

                return items;
            }

            public Dictionary<string, List<SelectListItem>> BuildDateTimeDuration()
            {
                if (_convention.Days is null)
                    return new Dictionary<string, List<SelectListItem>>();

                var items = new Dictionary<string, List<SelectListItem>>();
                foreach (var day in _convention.Days.OrderBy(x => x.Date))
                {
                    var timeSlots = _factory.GenerateTimeSlots(day.StartTime, day.EndTime, day.TimeSlotStrategy);
                    foreach (var option in timeSlots.StartTimeAndDurations)
                    {
                        var key = $"{day.Id},{option.Key.ToString("HH:mm", CultureInfo.CurrentCulture)}";
                        var values = option.Value.Select(x => new SelectListItem() {
                            Text = $"{x} שעות",
                            Value = x.ToString() });

                        items[key] = values.ToList();
                    }
                }

                return items;
            }

            public List<SelectListItem> BuildDurations()
            {
                if (_convention.Days is null)
                    return new List<SelectListItem>();

                var items = new List<SelectListItem>();
                var durations = new List<double>();
                foreach (var day in _convention.Days.OrderBy(x => x.Date))
                {
                    var timeSlots = _factory.GenerateTimeSlots(day.StartTime, day.EndTime, day.TimeSlotStrategy);
                    foreach (var duration in timeSlots.StartTimeAndDurations.Values)
                    {
                        durations.AddRange(duration);
                    }
                }

                durations = durations.Distinct().ToList();
                foreach (var duration in durations.OrderBy(x => x))
                {
                    items.Add(new SelectListItem($"{duration} שעות", duration.ToString()));
                }

                return items;
            }
        }

        public class ActorSystemState
        {

            public string DisplayConventionId { get; set; } = string.Empty;
            public string ManagersConventionId { get; set; } = string.Empty;
            public List<Activity> Activities { get; set; } = new List<Activity>();
            public List<AgeGroup> AgeGroups { get; set; } = new List<AgeGroup>();


            public Dictionary<string, string> ObjectIdAndValue = new Dictionary<string, string>();

            public string GetValue(string id)
            {
                if (id.IsEmptyString())
                    return id;

                id = id.Replace("\"", "");

                if (ObjectIdAndValue.ContainsKey(id))
                    return ObjectIdAndValue[id];

                return id;
            }

        }

        public class ActorParticipant
        {
            public string Id { get; set; } = string.Empty;
            public string FullName { get; set; } = string.Empty;
            public IList<SystemRoles> SystemRoles { get; set; } = new List<SystemRoles>();
        }

        public class ActorConventionState
        {
            public TimeSlotStrategy TimeStrategy { get; set; }
            public string ConventionId { get; set; } = string.Empty;
            public string ConventionName { get; set; } = string.Empty;
            public string Location { get; set; } = string.Empty;
            public string TagLine { get; set; } = string.Empty;
            public ConventionSettings Settings { get; set; } = new ConventionSettings();
            public List<Hall> Halls { get; set; } = new List<Hall>();
            public List<Ticket> Tickets { get; set; } = new List<Ticket>();
            public List<Day> Days { get; set; } = new List<Day>();
            public Day GetDayById(string dayId)
            {
                return Days.SingleOrDefault(x => x.Id == dayId);
            }
        }
        #endregion

        public class ActorDynamicContent
        {
            public IList<DynamicSponsorItem> Sponsors { get; set; } = new List<DynamicSponsorItem>();
            public IList<DynamicSlideItem> Slides { get; set; } = new List<DynamicSlideItem>();
            public IList<DynamicUpdateItem> UpdatesLimited { get; set; } = new List<DynamicUpdateItem>();
            public DynamicLocation Location { get; set; } = new DynamicLocation();
            public DynamicEnglish English { get; set; } = new DynamicEnglish();
        }
    }
}

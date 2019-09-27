using System.Collections.Generic;
using System.Linq;
using DragonCon.Modeling.Helpers;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.Events;
using DragonCon.Modeling.Models.HallsTables;
using DragonCon.Modeling.Models.System;
using DragonCon.Modeling.Models.Tickets;
using DragonCon.Modeling.TimeSlots;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DragonCon.Modeling.Models.Identities
{

    public interface IActor
    {
        Actor.ActorParticipant Participant { get; set; }
        Actor.ActorSystemState SystemState { get; set; }
        Actor.ActorDropDowns DropDowns { get; set; }
        
        bool HasSystemRole(SystemRoles role);
        bool HasConventionRole(ConventionRoles role);
    }

    public class Actor : IActor
    {
        public ActorParticipant Participant { get; set; }
        public ActorSystemState SystemState { get; set; }
        public Actor.ActorDropDowns DropDowns { get; set; }

        public bool HasSystemRole(SystemRoles role) => Participant.SystemRoles.Contains(role);
        public bool HasConventionRole(ConventionRoles role) => Participant.ConventionRoles.Contains(role);


        public class ActorDropDowns
        {
            private readonly ActorSystemState _state;
            private readonly IStrategyFactory _factory;

            public ActorDropDowns(IStrategyFactory factory, ActorSystemState state)
            {
                _state = state;
                _factory = factory;
            }

            public List<SelectListItem> BuildHalls()
            {
                if (_state.Halls is null)
                    return new List<SelectListItem>();

                var items = new List<SelectListItem>();
                foreach (var halls in _state.Halls.OrderBy(x => x.FirstTable))
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
                if (_state.AgeGroups == null)
                    return new List<SelectListItem>();

                return _state.AgeGroups
                    .OrderBy(x => x.Name)
                    .Select(x => new SelectListItem(x.GetDescription(), x.Id)).ToList();
            }

            public List<SelectListItem> Activities(bool addGeneralSelect = true)
            {
                var items = new List<SelectListItem>();
                if (_state.Activities == null)
                    return new List<SelectListItem>();

                foreach (var eventActivity in _state.Activities.OrderBy(x => x.Name))
                {
                    var group = new SelectListGroup {Name = eventActivity.Name};
                    if (addGeneralSelect)
                    {
                        items.Add(new SelectListItem
                        {
                            Value = eventActivity.Id,
                            Text = "כללי",
                            Group = group
                        });
                    }

                    foreach (var subActivity in eventActivity.SubActivities.OrderBy(x => x.Name))
                    {
                        var item = new SelectListItem
                        {
                            Value = subActivity.Id,
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
                if (_state.Days is null)
                    return new List<SelectListItem>();

                var items = new List<SelectListItem>();
                foreach (var day in _state.Days.OrderBy(x => x.Date))
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
                            Value = $"{starTime}",
                            Text = $"{starTime}",
                            Group = group
                        };
                        items.Add(item);
                    }
                }

                return items;
            }

            public List<SelectListItem> BuildDurations()
            {
                if (_state.Days is null)
                    return new List<SelectListItem>();

                var items = new List<SelectListItem>();
                var durations = new List<double>();
                foreach (var day in _state.Days.OrderBy(x => x.Date))
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
                    items.Add(new SelectListItem(duration.ToString(), duration.ToString()));
                }

                return items;
            }
        }
        public class ActorParticipant
        {
            public string Id { get; set; }
            public string FullName { get; set; }
            public IList<SystemRoles> SystemRoles { get; } = new List<SystemRoles>();
            public IList<ConventionRoles> ConventionRoles { get;set; } = new List<ConventionRoles>();
        }
        public class ActorSystemState
        {
            public long BuildMilliseconds { get; set; }
            public string ConventionId { get; set; }
            public string ConventionName { get; set; }
            public string Location { get; set; }
            public string TagLine { get; set; }
            public List<Hall> Halls { get; set; } = new List<Hall>();
            public List<Ticket> Tickets { get; set; } = new List<Ticket>();
            public List<Day> Days { get; set; } = new List<Day>();
            public List<Activity> Activities { get; set; } = new List<Activity>();
            public List<AgeGroup> AgeGroups { get; set; } = new List<AgeGroup>();
            public SystemConfiguration Configurations { get; set; }
            public bool HasActiveConvention => ConventionId.IsNotEmptyString();
        }
    }
}

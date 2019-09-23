using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DragonCon.Logical.Factories;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.Events;
using DragonCon.Modeling.Models.HallsTables;
using Microsoft.AspNetCore.Mvc.Rendering;
using NodaTime;

namespace DragonCon.Features.Management.Events
{
    public class EventCreateUpdateViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ConventionDayId { get;set; }
        public string SystemId { get;set; }

        public List<string> GameMasterIds { get; set; }

        public EventStatus Status { get; set; }
        public SizeRestriction Size { get;set; }
        public string AgeRestrictionId { get; set; }

        public DateTime? StartTime { get; set; }
        public int? Duration { get; set; }

        public List<string> Tags { get;set; }
        public string Table { get; set; }
        
        public string Description { get; set; }
        public string SpecialRequests { get;set; }

        public bool IsSpecialPrice { get; set; }
        public double? SpecialPrice { get; set; }


        #region Selectors

        public Dictionary<Day, TimeSlotOptions> GetDayOptions()
        {
            var result = new Dictionary<Day, TimeSlotOptions>();
            var factory = new StrategyFactory();
            foreach (var day in Days)
            {
                result.Add(day, factory.TimeSlots(day.StartTime, day.EndTime, day.TimeSlotStrategy));
            }

            return result;
        }


        public List<AgeGroup> AgeGroups { get; set; }
        public List<SelectListItem> GetAgeRestrictionDropDown
        {
            get
            {
                if (AgeGroups == null)
                    return new List<SelectListItem>();

                var items = new List<SelectListItem>();
                foreach (var age in AgeGroups.OrderBy(x => x.Name))
                {
                    var item = new SelectListItem
                    {
                        Value = age.Id,
                        Text = age.GetDescription(),
                    };
                    items.Add(item);
                }

                return items;
            }
        }

        public List<Activity> Activities { get; set; }
        public List<SelectListItem> GetActivitiesDropDown
        {
            get
            {
                if (Activities == null)
                    return new List<SelectListItem>();

                var items = new List<SelectListItem>();
                foreach (var eventActivity in Activities.OrderBy(x => x.Name))
                {
                    var group = new SelectListGroup {Name = eventActivity.Name};
                    items.Add(new SelectListItem
                    {
                        Value = $"{eventActivity.Id},",
                        Text = "כללי",
                        Group = group
                    });

                    foreach (var system in eventActivity.SubActivities.OrderBy(x => x.Name))
                    {
                        var item = new SelectListItem
                        {
                            Value = $"{eventActivity.Id},{system.Id}",
                            Text = system.Name,
                            Group = group
                        };
                        items.Add(item);
                    }
                }

                return items;
            }
        }
        public IEnumerable<Day> Days { get; set; }
        public List<SelectListItem> GetDaysDropDown
        {
            get
            {
                if (Days == null)
                    return new List<SelectListItem>();

                var items = new List<SelectListItem>();
                foreach (var day in Days.OrderBy(x => x.Date))
                {
                    var item = new SelectListItem
                    {
                        Value = day.Id,
                        Text = day.GetDescription()
                    };
                    items.Add(item);
                }

                return items;
            }
        }

        public IEnumerable<Hall> Halls { get; set; }
        public List<SelectListItem> GetHallsDropdown
        {
            get
            {
                if (Halls == null)
                    return new List<SelectListItem>();

                var items = new List<SelectListItem>();
                foreach (var halls in Halls.OrderBy(x => x.FirstTable))
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
        }

        #endregion
    }
}

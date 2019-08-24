using System.Collections.Generic;
using System.Linq;
using DragonCon.Features.Shared;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Events;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DragonCon.Features.Management.Events
{
    public class EventsManagementViewModel : IDisplayPaginationViewModel
    {
        public class Filters {
            public string Activity { get; set; }
            public string Status { get; set; }
        }

        public IDisplayPagination Pagination { get; set; }
        public Filters ActiveFilters { get; set; }
        public string ActiveConvention { get;set; } 
        public List<EventWrapper> Events { get; set; } = new List<EventWrapper>();
        public List<AgeGroup> AgeGroups { get; set; }

        public List<SelectListItem> GetAgeRestrictionDropDown
        {
            get
            {
                return AgeGroups.Select(x => new SelectListItem(x.Name, x.Id)).ToList();
            }
        }

        public List<Activity> Activities { get; set; }
        public List<SelectListItem> GetActivitiesDropDown
        {
            get
            {
                var items = new List<SelectListItem>();
                foreach (var eventActivity in Activities.OrderBy(x => x.Name))
                {
                    var group = new SelectListGroup {Name = eventActivity.Name};
                    items.Add(new SelectListItem
                    {
                        Value = eventActivity.Id,
                        Text = "הכל",
                        Group = group
                    });

                    foreach (var system in eventActivity.SubActivities.OrderBy(x => x.Name))
                    {
                        var item = new SelectListItem
                        {
                            Value = system.Id,
                            Text = system.Name,
                            Group = group
                        };
                        items.Add(item);
                    }
                }

                return items;
            }
        }
    }
}

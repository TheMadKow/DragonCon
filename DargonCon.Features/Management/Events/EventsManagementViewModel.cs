using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DragonCon.Features.Shared;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.Events;
using DragonCon.Modeling.Models.System;
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
        public List<ConEventWrapper> Events { get; set; } = new List<ConEventWrapper>();
        public List<AgeRestriction> AgeRestrictions { get; set; }

        public List<SelectListItem> GetAgeRestrictionDropDown
        {
            get
            {
                return AgeRestrictions.Select(x => new SelectListItem(x.Name, x.Id)).ToList();
            }
        }

        public List<EventActivity> Activities { get; set; }
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

                    foreach (var system in eventActivity.ActivitySystems.OrderBy(x => x.Name))
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

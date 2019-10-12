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
            public string AgeGroup { get; set; }
            public string DayAndTime { get; set; }
            public double Duration { get; set; }
        }

        public IDisplayPagination Pagination { get; set; }
        public Filters ActiveFilters { get; set; }
        public List<EventWrapper> Events { get; set; } = new List<EventWrapper>();
    }
}

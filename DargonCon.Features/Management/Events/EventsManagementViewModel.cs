using System;
using System.Collections.Generic;
using System.Text;
using DragonCon.Features.Shared;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.Events;
using DragonCon.Modeling.Models.System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DragonCon.Features.Management.Events
{
    public class EventsManagementViewModel : IDisplayPaginationViewModel
    {
        public class Filters {
    
            public List<SelectListItem> Activities { get; set; }
            public List<SelectListItem> Systems { get; set; }
            public List<SelectListItem> ConDays { get; set; }
            public List<SelectListItem> Statuses { get; set; }
            public List<SelectListItem> GameMasters { get; set; }


            public string Activity { get; set; }
            public string System { get; set; }
            public string ConDay { get; set; }
            public string GameMaster { get; set; }
            public EventStatus Status { get; set; }
        }


        public IDisplayPagination Pagination { get; set; }
        public Filters ActiveFilters { get; set; }
        public string ActiveConvention { get;set; } 
        public List<ConEventWrapper> Events { get; set; } = new List<ConEventWrapper>();
    }
}

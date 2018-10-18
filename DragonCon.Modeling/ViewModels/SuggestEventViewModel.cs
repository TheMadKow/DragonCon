using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Events;
using Microsoft.AspNetCore.Mvc.Rendering;
using NodaTime;

namespace DragonCon.Modeling.ViewModels
{
    public class SuggestEventViewModel
    {
        public string CreatorId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Description { get; set; }

        public string Requests { get; set; }
        public List<string> HelperIds { get; set; }

        [Required]
        public string DayId { get; set; }
        
        [Required]
        public LocalTime StartTime { get; set; }
        
        [Required]
        public Period Period { get; set; }

        [Required]
        public string ActivityId { get; set; }
        
        public string SystemId { get; set; }

        [Required]
        public string AgeRestrictionId { get; set; }
        
        [Required]
        public SizeRestriction SizeRestrictions { get; set; }

        public List<string> Tags { get; set; }

        // Drop Downs
        public List<SelectListItem> StartTimes { get; set; }
        public List<SelectListItem> DurationTimes { get; set; }

        public List<SelectListItem> Activities { get; set; }
        public Dictionary<string, List<SelectListItem>> ActivityAndSystems { get; set; }

        public List<SelectListItem> AgeRestrictions { get; set; }

    }
}
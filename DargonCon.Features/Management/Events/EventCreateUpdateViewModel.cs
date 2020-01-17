using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DragonCon.Features.Shared;
using DragonCon.Modeling.Helpers;
using DragonCon.Modeling.Models.Events;

namespace DragonCon.Features.Management.Events
{
    public class EventCreateUpdateViewModel
    {
        public string? Id { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "חובה להזין שם אירוע")]
        public string Name { get; set; }
        
        [Required(AllowEmptyStrings = false, ErrorMessage = "חובה לבחור פעילות")]
        public string ActivitySelector { get;set; }
        public string ActivityId => ActivitySelector?.SplitTuples().Major ?? string.Empty;
        public string SubActivityId => ActivitySelector?.SplitTuples().Minor ?? string.Empty;

        public string? HallTableSelector { get; set; }
        public string? HallId => HallTableSelector?.SplitTuples().Major ?? string.Empty;
        public string? HallTable => HallTableSelector?.SplitTuples().Minor ?? string.Empty;

        public List<string> HostIds { get; set; } = new List<string>();
        public List<Select2Option> HostsSelected { get; set; } = new List<Select2Option>();

        public EventStatus Status { get; set; }
        public uint MinSize { get;set; }
        public uint MaxSize { get;set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "חובה לבחור קבוצה גיל")]
        public string AgeId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "חובה לבחור שעת התחלה")]
        public string StartTimeSelector { get; set; }
        public string ConventionDayId => StartTimeSelector?.SplitTuples().Major ?? string.Empty;
        public string StartTime => StartTimeSelector?.SplitTuples().Minor ?? string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "חובה לבחור משך אירוע")]
        public double? Duration { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "חובה להזין תיאור אירוע")]
        public string Description { get; set; }
        public string? SpecialRequests { get;set; }

        public bool IsFree { get; set; }
        public double? ExtraCharge { get; set; }
    }
}

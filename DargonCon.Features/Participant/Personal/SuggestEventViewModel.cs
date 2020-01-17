using System.ComponentModel.DataAnnotations;
using DragonCon.Modeling.Helpers;
using DragonCon.Modeling.Models.Common;

namespace DragonCon.Features.Participant.Personal
{
    public class SuggestEventViewModel
    {

        [Required(AllowEmptyStrings = false, ErrorMessage = "חובה להזין שם אירוע")]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "חובה להזין תיאור אירוע")]
        public string Description { get; set; }

        public string? Requests { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "חובה לבחור שעת התחלה")]
        public string StartTimeSelector { get; set; }
        public string ConventionDayId => StartTimeSelector?.SplitTuples().Major ?? string.Empty;
        public string StartTime => StartTimeSelector?.SplitTuples().Minor ?? string.Empty;


        [Range(1, int.MaxValue, ErrorMessage = "חובה לבחור משך אירוע")]
        public double Duration { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "חובה לבחור פעילות")]
        public string ActivitySelector { get; set; }
        public string ActivityId => ActivitySelector?.SplitTuples().Major ?? string.Empty;
        public string SubActivityId => ActivitySelector?.SplitTuples().Minor ?? string.Empty;

        [Required(AllowEmptyStrings = false, ErrorMessage = "חובה לבחור קבוצה גיל")]

        public string AgeRestrictionId { get; set; }

        public uint Min { get; set; } = 1;
        public uint Max { get; set; } = 6;
    }
}
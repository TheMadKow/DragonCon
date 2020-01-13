using System.ComponentModel.DataAnnotations;
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
        public string StartTime { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "חובה לבחור משך אירוע")]
        public double Duration { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "חובה לבחור פעילות")]
        public string ActivityId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "חובה לבחור קבוצה גיל")]

        public string AgeRestrictionId { get; set; }
        public uint? Min { get; set; }
        public uint? Max { get; set; }
    }
}
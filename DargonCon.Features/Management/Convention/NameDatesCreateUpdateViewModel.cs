using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Conventions;

namespace DragonCon.Features.Management.Convention
{
    public class NameDatesCreateUpdateViewModel
    {
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "חובה להזין שם לכנס")]
        public string Name { get; set; }
        public string TagLine { get; set; }

        public TimeSlotStrategy TimeStrategy { get; set; }

        public List<DaysViewModel> Days {get; set; }
    }

    public class DaysViewModel
    {
        public DaysViewModel(){}
        
        public DaysViewModel(Day day)
        {
            IsDeleted = false;

            Id = day.Id;
            Date = day.Date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            From = $"{day.StartTime.Hour}:{day.StartTime.Minute}";
            To = $"{day.EndTime.Hour}:{day.EndTime.Minute}";
        }

        public string Id { get; set; }


        public bool IsDeleted {get; set; }
        public string Date {get; set; }
        public string From { get; set; }
        public string To { get; set; }

    }
}

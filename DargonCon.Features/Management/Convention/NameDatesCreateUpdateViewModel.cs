using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using DragonCon.Modeling.Models.Conventions;

namespace DragonCon.Features.Management.Convention
{
    public class NameDatesCreateUpdateViewModel
    {
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "חובה להזין שם לכנס")]
        public string Name { get; set; }

        public List<DaysViewModel> Days {get; set; }
    }

    public class DaysViewModel
    {
        public DaysViewModel(){}
        
        public DaysViewModel(ConDayWrapper day)
        {
            IsDeleted = false;

            Id = day.Id;
            Date = day.Date.ToDateTimeUnspecified();
            From = new DateTime(1, 1, 1, day.StartTime.Hour, day.StartTime.Minute, 0);
            To = new DateTime(1, 1, 1, day.EndTime.Hour, day.EndTime.Minute, 0);
        }

        public string Id { get; set; }


        public bool IsDeleted {get; set; }
        public DateTime Date {get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }

    }
}

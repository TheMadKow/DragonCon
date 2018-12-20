using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using DragonCon.Modeling.Models.Tickets;
using NodaTime;

namespace DragonCon.Features.Management.Convention
{
    public class ConventionCreateUpdateViewModel
    {
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "חובה להזין שם לכנס")]
        public string Name { get; set; }

        public List<DaysViewModel> Days {get; set; }
        public List<TicketViewModel> Tickets {get; set; }
    }

    public class DaysViewModel
    {
        public bool IsDeleted {get; set; }
        public DateTime Date {get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }

    }

    public class TicketViewModel
    {
        public bool IsDeleted {get; set; }
        public string Name { get;set; }
        public List<LocalDate> Dates {get;set;}
        public TicketLimitation Limitation { get; set; } = TicketLimitation.NotLimited;
    }
}

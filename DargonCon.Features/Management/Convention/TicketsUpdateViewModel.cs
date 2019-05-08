using System.Collections.Generic;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.Tickets;
using NodaTime;

namespace DragonCon.Features.Management.Convention
{
    public class TicketsUpdateViewModel
    {
        public string ConventionId { get; set; }
        public List<TicketViewModel> Tickets { get; set; }
        public List<DaysViewModel> AvailableDays { get; set; }
    }

    public class TicketViewModel
    {
        public TicketViewModel(TicketWrapper ticket)
        {
            Id = ticket.Id;
            Name = ticket.Name;
            Price = ticket.Price;
            TransactionCode = ticket.TransactionCode;
            NumOfActivities = ticket.ActivitiesAllowed;
            TicketLimitation = ticket.TicketLimitation;
            Days = ticket.Days;
            IsDeleted = false;
        }

        public string Id { get; set; }
        public string Name { get;set; }
        public double Price { get; set; }
        public string TransactionCode { get; set; }
        public int? NumOfActivities { get; set; }
        public List<ConDayWrapper> Days { get; set; }
        public bool IsUnlimited => NumOfActivities == null;
        public bool IsDeleted { get; set; }
        public TicketLimitation TicketLimitation { get; set; }

    }
}
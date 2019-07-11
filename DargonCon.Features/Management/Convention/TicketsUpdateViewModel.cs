using System.Collections.Generic;
using DragonCon.Modeling.Models.Tickets;

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
        public TicketViewModel(){}

        public TicketViewModel(TicketWrapper ticket)
        {
            Id = ticket.Id;
            Name = ticket.Name;
            Price = ticket.Price;
            TransactionCode = ticket.TransactionCode;
            TicketType = ticket.TicketType;
            Days = ticket.Days;
            IsDeleted = false;

            NumOfActivities = ticket.ActivitiesAllowed;
            IsLimited = ticket.ActivitiesAllowed.HasValue;
        }

        public string Id { get; set; }
        public string Name { get;set; }
        public double Price { get; set; }
        public string TransactionCode { get; set; }
        public int? NumOfActivities { get; set; }
        public List<string> Days { get; set; }
        public bool IsLimited { get; set; }
        public bool IsDeleted { get; set; }
        public TicketType TicketType { get; set; }

    }
}
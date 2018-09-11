using System.Collections.Generic;
using DragonCon.Modeling.Models.Convention;
using DragonCon.Modeling.Models.Tickets;

namespace DragonCon.Modeling.Models.Wrappers
{
    public class TicketWrapper : Ticket
    {
        public TicketWrapper()
        {

        }

        public TicketWrapper(Ticket other)
        {
            this.Id = other.Id;
            this.DayIds = other.DayIds;
            this.Name = other.Name;
            this.Price = other.Price;
            this.TransactionCode = other.TransactionCode;
            this.ActivitiesAllowed = other.ActivitiesAllowed;
            this.UnlimitedActivities = other.UnlimitedActivities;

        }

        public List<ConventionDay> Days { get; set; }
    }
}
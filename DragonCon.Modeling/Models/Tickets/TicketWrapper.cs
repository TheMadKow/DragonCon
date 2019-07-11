using System.Collections.Generic;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Conventions;

namespace DragonCon.Modeling.Models.Tickets
{
    public class TicketWrapper : Wrapper<ITicket>
    {

        public TicketWrapper()
        {
            Model = new Ticket();
        }

        public TicketWrapper(Ticket ticket)
        {
            Model = ticket;
        }

        public string Id
        {
            get => Model.Id;
            set => Model.Id = value;
        }

        public string Name
        {
            get => Model.Name;
            set => Model.Name = value;
        }

        public double Price
        {
            get => Model.Price;
            set => Model.Price = value;
        }

        public string TransactionCode
        {
            get => Model.TransactionCode;
            set => Model.TransactionCode = value;
        }

        public int? ActivitiesAllowed
        {
            get => Model.ActivitiesAllowed;
            set => Model.ActivitiesAllowed = value;
        }


        public List<string> Days
        {
            get => Model.DayIds;
            set => Model.DayIds = value;
        }

        public TicketType TicketType
        {
            get => Model.TicketType;
            set => Model.TicketType = value;
        }

        public bool IsUnlimited => ActivitiesAllowed == null;
    }
}
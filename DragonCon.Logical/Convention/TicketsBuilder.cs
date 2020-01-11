using System;
using System.Collections.Generic;
using System.Linq;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.Tickets;

namespace DragonCon.Logical.Convention
{
    public class TicketsBuilder : BuilderBase<Ticket>
    {
        public TicketsBuilder(ConventionBuilder parent, ConventionWrapper convention) :
            base(parent, convention)
        {

        }


        public ConventionBuilder RemoveTicket(string ticketId)
        {
            ThrowIfTicketNotExists(ticketId);
            var removedTicket = this[ticketId];
            Convention.Tickets.Remove(removedTicket);
            Parent.DeletedEntityIds.Add(ticketId);
            return Parent;
        }

        public ConventionBuilder AddTicket(
            string name, 
            List<string> dayIds,
            int? numOfActivities,
            double price,
            TicketType type)
        {
            ThrowIfTicketNameEmpty(name);
            ThrowIfTicketNameExists(name);

            foreach (var date in dayIds)
            {
                if (Parent.Days[date] == null)
                    throw new Exception("Convention Day does not exist");
            }

            var ticket = new Ticket
            {
                Name = name,
                DayIds =  dayIds, 
                TicketType = type,
                Price = price,
                ActivitiesAllowed = numOfActivities
            };

            Convention.Tickets.Add(ticket);
            return Parent;
        }


        private void ThrowIfTicketNameExists(string ticketName)
        {
            if (Convention.Tickets.Any(x => x.Name == ticketName))
                throw new Exception("Ticket name already exists");
        }

        private void ThrowIfTicketNotExists(string ticketId)
        {
            if (KeyExists(ticketId) == false)
                throw new Exception("Ticket does not exists");
        }


        private void ThrowIfTicketNameEmpty(string ticketName)
        {
            if (string.IsNullOrEmpty(ticketName) || string.IsNullOrWhiteSpace(ticketName))
                throw new Exception("Empty ticket name");   
        }

        public ConventionBuilder UpdateTicket(string ticketId,
            string name, 
            List<string> dayIds,
            int? numOfActivities,
            double price, TicketType type)
        {
            ThrowIfTicketNotExists(ticketId);
            ThrowIfPriceIsNotValid(price);

            var ticket = this[ticketId];
            ticket.Name = name;
            ticket.Price = price;
            ticket.ActivitiesAllowed = numOfActivities;
            ticket.TicketType = type;
            ticket.DayIds = dayIds;

            return Parent;
        }

        private void ThrowIfPriceIsNotValid(double price)
        {
            if (price < 0)
                throw new Exception("Cannot set negative price");
        }
    }
}
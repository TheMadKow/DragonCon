using System;
using System.Collections.Generic;
using System.Linq;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.Tickets;

namespace DragonCon.Logical.Convention
{
    public class TicketsBuilder
    {
        private readonly ConventionBuilder _parent;
        private readonly ConventionWrapper _convention;
        public TicketWrapper this[string key]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(key))
                    return null;

                return _convention.Tickets.SingleOrDefault(x => x.Id == key);
            }
        }

        public bool IsTicketExists(string ticketId)
        {
            return this[ticketId] != null;
        }

        public TicketsBuilder(ConventionBuilder parent, ConventionWrapper convention)
        {
            _convention = convention;
            _parent = parent;
        }

        //public ConventionBuilder AddTicket(string ticketName, params LocalDate[] localDates)
        //{
        //    return AddTicket(TicketType.NotLimited, ticketName, localDates.ToList());
        //}

        //public ConventionBuilder AddLimitedTicket(TicketType role, string ticketName, params LocalDate[] localDates)
        //{
        //    return AddTicket(role, ticketName, localDates.ToList());
        //}

        
        public ConventionBuilder RemoveTicket(string ticketId)
        {
            ThrowIfTicketNotExists(ticketId);
            var removedTicket = this[ticketId];
            _convention.Tickets.Remove(removedTicket);
            _parent.DeletedEntityIds.Add(ticketId);
            return _parent;
        }

        public ConventionBuilder AddTicket(
            string name, 
            List<string> dayIds,
            string code, int? numOfActivities,
            double price,
            TicketType type)
        {
            ThrowIfTicketNameEmpty(name);
            ThrowIfTicketNameExists(name);

            foreach (var date in dayIds)
            {
                if (_parent.Days[date] == null)
                    throw new Exception("Convention Day does not exist");
            }

            var ticket = new TicketWrapper
            {
                Name = name,
                Days =  dayIds, 
                TicketType = type,
                TransactionCode = code,
                Price = price,
                ActivitiesAllowed = numOfActivities
            };

            _convention.Tickets.Add(ticket);
            return _parent;
        }


        private void ThrowIfTicketNameExists(string ticketName)
        {
            if (_convention.Tickets.Any(x => x.Name == ticketName))
                throw new Exception("Ticket name already exists");
        }

        private void ThrowIfTicketNotExists(string ticketId)
        {
            if (IsTicketExists(ticketId) == false)
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
            string code, int? numOfActivities,
            double price, TicketType type)
        {
            ThrowIfTicketNotExists(ticketId);
            ThrowIfPriceIsNotValid(price);

            var ticket = this[ticketId];
            ticket.Name = name;
            ticket.TransactionCode = code;
            ticket.Price = price;
            ticket.ActivitiesAllowed = numOfActivities;
            ticket.TicketType = type;
            ticket.Days = dayIds;

            return _parent;
        }

        private void ThrowIfPriceIsNotValid(double price)
        {
            if (price < 0)
                throw new Exception("Cannot set negative price");
        }
    }
}
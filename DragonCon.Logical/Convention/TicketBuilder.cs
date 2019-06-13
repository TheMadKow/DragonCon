using System;
using System.Collections.Generic;
using System.Linq;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.Tickets;
using NodaTime;

namespace DragonCon.Logical.Convention
{
    public class TicketBuilder
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

        public TicketBuilder(ConventionBuilder parent, ConventionWrapper convention)
        {
            _convention = convention;
            _parent = parent;
        }

        public ConventionBuilder AddTicket(string ticketName, params LocalDate[] localDates)
        {
            return AddTicket(TicketLimitation.NotLimited, ticketName, localDates.ToList());
        }

        public ConventionBuilder AddLimitedTicket(TicketLimitation role, string ticketName, params LocalDate[] localDates)
        {
            return AddTicket(role, ticketName, localDates.ToList());
        }


        public ConventionBuilder AddTicket(TicketLimitation role, string ticketName, List<LocalDate> localDates)
        {
            ThrowIfTicketNameEmpty(ticketName);
            ThrowIfTicketNameExists(ticketName);

            foreach (var date in localDates)
            {
                if (!_convention.Days.ContainsKey(date))
                    throw new Exception("Ticket-Day does not exists");
            }

            var ticket = new TicketWrapper()
            {
                Name = ticketName,
                Days = localDates.Select(x => _parent.Days[x]).ToList(),
                TicketLimitation = role
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
            string name, string code, int? numOfActivities,
            double price, TicketLimitation limitation)
        {
            ThrowIfTicketNotExists(ticketId);
            //TODO ThrowIfActivityInvalid(numOfActivities.Value);
            ThrowIfPriceIsNotValid(price);

            var ticket = this[ticketId];
            ticket.Name = name;
            ticket.TransactionCode = code;
            //TODO days
            ticket.Price = price;
            ticket.ActivitiesAllowed = numOfActivities;
            ticket.TicketLimitation = limitation;

            return _parent;
        }

        private void ThrowIfPriceIsNotValid(double price)
        {
            if (price < 0)
                throw new Exception("Cannot set negative price");
        }
    }
}
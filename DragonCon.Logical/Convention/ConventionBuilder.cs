using System;
using System.Collections.Generic;
using System.Linq;
using DragonCon.Logical.Gateways;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.HallsTables;
using DragonCon.Modeling.Models.Tickets;
using NodaTime;

namespace DragonCon.Logical.Convention
{
    public class ConventionBuilder
    {
        public enum Migrate
        {
            Days,
            Tickets,
            Halls
        }

        private ConventionWrapper _convention = null;
        private readonly IConventionBuilderGateway _gateway;

        public List<string> DeletedEntityIds { get; set; } = new List<string>();

        public DaysBuilder Days { get; private set; }
        public TicketsBuilder Tickets { get; set; }
        public HallsBuilder Halls { get; set; }
        public string ConventionName => _convention.Inner.Name;

        public ConventionBuilder(IConventionBuilderGateway gateway)
        {
            _gateway = gateway;
        }

        public ConventionBuilder NewConvention(string name)
        {
            _convention = new ConventionWrapper
            {
                Halls = new List<Hall>(),
                Tickets = new List<Ticket>(),
                Days = new List<Day>()
            };
            _convention.Inner.Name = name;
            CreateSubBuilders();
            return this;
        }

        public ConventionBuilder SetTimeSlotStrategy(TimeSlotStrategy strategy)
        {
            _convention.Inner.TimeStrategy = strategy;
            return this;
        }

        public ConventionBuilder ChangeName(string name)
        {
            ThrowIfStringIsEmpty(name);
            _convention.Inner.Name = name;
            return this;
        }

        public ConventionBuilder AddTagLine(string tagLine)
        {
            _convention.Inner.TagLine = tagLine;
            return this;
        }

        private void ThrowIfStringIsEmpty(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new Exception("New name cannot be empty");
        }

        public ConventionBuilder LoadConvention(string id)
        {
            _convention = _gateway.GetConventionWrapper(id);
            CreateSubBuilders();
            return this;
        }

        public ConventionBuilder NewFromOldConvention(string conventionName, ConventionWrapper oldCon, params Migrate[] migrations)
        {
            ThrowIfRequestInvalid(migrations);
            NewConvention(conventionName);
            if (migrations.Contains(Migrate.Halls))
                MigrateHalls(oldCon);
            if (migrations.Contains(Migrate.Days))
                MigrateDays(oldCon);
            if (migrations.Contains(Migrate.Tickets))
                MigrateTickets(oldCon);
            return this;
        }

        private void MigrateTickets(ConventionWrapper oldCon)
        {
            foreach (var ticket in oldCon.Tickets)
            {
                //Tickets.AddLimitedTicket(ticket.Value.TicketType, ticket.Key, ticket.Value.Days.Select(x => x.Date).ToArray());
                //Tickets.SetTicketPrice(ticket.Key, ticket.Value.Price);
                //Tickets.SetTransactionCode(ticket.Key, ticket.Value.TransactionCode);

                //if (ticket.Value.IsUnlimited)
                //    Tickets.SetUnlimitedActivities(ticket.Key);
                //else if (ticket.Value.ActivitiesAllowed != null)
                //    Tickets.SetNumberOfActivities(ticket.Key, ticket.Value.ActivitiesAllowed.Value);
            }
        }
        private void ThrowIfRequestInvalid(params Migrate[] migrations)
        {
            if (migrations.Contains(Migrate.Tickets) && !migrations.Contains(Migrate.Days))
                throw new Exception("Cannot migrate tickets without days");
        }


        private void MigrateHalls(ConventionWrapper oldCon)
        {
            foreach (var hall in _convention.Halls)
            {
                var prevHall = hall;
                Halls.AddHall(prevHall.Name,
                    prevHall.Description,
                    prevHall.FirstTable,
                    prevHall.LastTable);
            }
        }

        private void MigrateDays(ConventionWrapper oldCon)
        {
            foreach (var day in oldCon.Days)
            {
                //Days.AddDay(day.Key, day.Value.StartTime, day.Value.EndTime);
                //Days.SetTimeSlotStrategy(day.Key, day.Value.TimeSlotStrategy);
            }
        }

        private void CreateSubBuilders()
        {
            Days = new DaysBuilder(this, _convention);
            Tickets = new TicketsBuilder(this, _convention);
            Halls = new HallsBuilder(this, _convention);
        }


        public ConventionBuilder Save()
        {
            _gateway.StoreConvention(_convention, DeletedEntityIds);
            return this;
        }

    }
}

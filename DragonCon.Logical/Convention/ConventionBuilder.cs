using System;
using System.Collections.Generic;
using System.Linq;
using DragonCon.Logical.Gateways;
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

        public DaysBuilder Days { get; private set; }
        public TicketBuilder Tickets { get; set; }
        public HallsBuilder Halls { get; set; }
        public string ConventionName => _convention.Name;

        public ConventionBuilder(IConventionBuilderGateway gateway)
        {
            _gateway = gateway;
        }

        public ConventionBuilder NewConvention(string name)
        {
            _convention = new ConventionWrapper
            {
                Name = name,
                Days = new Dictionary<LocalDate, ConDayWrapper>(),
                NameAndHall = new Dictionary<string, HallWrapper>(),
                NameAndTickets = new Dictionary<string, TicketWrapper>()
            };
            CreateSubBuilders();
            return this;
        }

        public ConventionBuilder ChangeName(string name)
        {
            ThrowIfStringIsEmpty(name);
            _convention.Name = name;
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
            foreach (var ticket in oldCon.NameAndTickets)
            {
                Tickets.AddLimitedTicket(ticket.Value.TicketLimitation, ticket.Key, ticket.Value.Days.Select(x => x.Date).ToArray());
                Tickets.SetTicketPrice(ticket.Key, ticket.Value.Price);
                Tickets.SetTransactionCode(ticket.Key, ticket.Value.TransactionCode);

                if (ticket.Value.IsUnlimited)
                    Tickets.SetUnlimitedActivities(ticket.Key);
                else if (ticket.Value.ActivitiesAllowed != null)
                    Tickets.SetNumberOfActivities(ticket.Key, ticket.Value.ActivitiesAllowed.Value);
            }
        }
        private void ThrowIfRequestInvalid(params Migrate[] migrations)
        {
            if (migrations.Contains(Migrate.Tickets) && !migrations.Contains(Migrate.Days))
                throw new Exception("Cannot migrate tickets without days");
        }


        private void MigrateHalls(ConventionWrapper oldCon)
        {
            foreach (var hall in _convention.NameAndHall)
            {
                Halls.AddHall(hall.Key, hall.Value.Description);
                var newTables = hall.Value.Tables.Select(x => new Table(hall.Key, x.Name)
                {
                    Notes = x.Notes
                });
                Halls.SetHallTables(hall.Key, newTables);
            }
        }

        private void MigrateDays(ConventionWrapper oldCon)
        {
            foreach (var day in oldCon.Days)
            {
                Days.AddDay(day.Key, day.Value.StartTime, day.Value.EndTime);
                Days.SetTimeSlotStrategy(day.Key, day.Value.TimeSlotStrategy);
            }
        }

        private void CreateSubBuilders()
        {
            Days = new DaysBuilder(this, _convention);
            Tickets = new TicketBuilder(this, _convention);
            Halls = new HallsBuilder(this, _convention);
        }


        public ConventionBuilder Save()
        {
            if (_convention.CreateTimeStamp == Instant.MinValue)
                _convention.CreateTimeStamp = SystemClock.Instance.GetCurrentInstant();

            _convention.UpdateTimeStamp = SystemClock.Instance.GetCurrentInstant();

            _gateway.StoreConvention(_convention);
            return this;
        }

    }
}

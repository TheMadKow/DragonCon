using System;
using System.Collections.Generic;
using System.Linq;
using DragonCon.Modeling.Gateways;
using DragonCon.Modeling.Models.Convention;
using DragonCon.Modeling.Models.Wrappers;
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
        private readonly IConventionGateway _gateway;

        public DaysBuilder Days { get; private set; }
        public TicketBuilder Tickets { get; set; }
        public HallsBuilder Halls { get; set; }

        public ConventionBuilder(IConventionGateway gateway)
        {
            _gateway = gateway;
        }

        public ConventionBuilder NewConvention(string name)
        {
            _convention = new ConventionWrapper
            {
                Name = name,
                Days = new Dictionary<LocalDate, ConventionDay>(),
                NameAndHall = new Dictionary<string, Hall>(),
                NameAndTickets = new Dictionary<string, TicketWrapper>()
            };
            CreateSubBuilders();
            return this;
        }


        public ConventionBuilder UpdateConvention(string id)
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
                Tickets.AddTicket(ticket.Key, ticket.Value.Days.Select(x => x.Date).ToList());
                Tickets.SetTicketPrice(ticket.Key, ticket.Value.Price);
                Tickets.SetTransactionCode(ticket.Key, ticket.Value.TransactionCode);
                Tickets.SetUnlimitedActivities(ticket.Key, ticket.Value.UnlimitedActivities);
                Tickets.SetNumberOfActivities(ticket.Key, ticket.Value.ActivitiesAllowed);
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
                Halls.SetHallTables(hall.Key, hall.Value.Tables.ToArray());
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
        }


        public ConventionBuilder Save()
        {
            _gateway.StoreConvention(_convention);
            return this;
        }

        public ConventionWrapper GetConvention() => _convention;


    }
}

using System.Collections.Generic;
using System.Linq;
using DragonCon.Modeling.Gateways;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.HallsTables;
using DragonCon.Modeling.Models.Identities;
using DragonCon.Modeling.Models.Tickets;
using Raven.Client.Documents.Session;

namespace DragonCon.RavenDB.Gateways
{
    public class ConventionGateway : IConventionGateway
    {
        private readonly StoreHolder _holder;

        public ConventionGateway()
        {

        }

        public ConventionGateway(StoreHolder holder)
        {
            _holder = holder;
        }

        public ConventionWrapper GetConventionWrapper(string id)
        {
            using (var session = _holder.Store.OpenSession())
            {
                Convention convention = session
                    .Include<Convention>(x => x.DayIds)
                    .Include<Convention>(x => x.HallIds)
                    .Include<Convention>(x => x.TicketIds)
                    .Load<Convention>(id);

                return new ConventionWrapper()
                {
                    Name = convention.Name,
                    Id = convention.Id,
                    Days = session.Load<ConventionDay>(convention.DayIds).Select(x => x.Value).ToDictionary(x => x.Date, x => x),
                    NameAndHall = session.Load<Hall>(convention.HallIds).Select(x => x.Value).ToDictionary(x => x.Name, x => x),
                    NameAndTickets = session.Load<Ticket>(convention.TicketIds).Select(x => x.Value).ToDictionary(x => x.Name, x => new TicketWrapper(x)),
                };
            }
        }

        public virtual void StoreConvention(ConventionWrapper convention)
        {
            using (var session = _holder.Store.OpenSession())
            {
                var conventionData = session.Load<Convention>(convention.Id) ?? new Convention();

                StoreConvDays(convention, conventionData, session);
                StoreConvHalls(convention, conventionData, session);
                StoreConvTickets(convention, conventionData, session);

                conventionData.Name = convention.Name;
                session.Store(conventionData);
                session.SaveChanges();
            }
        }

        private void StoreConvTickets(ConventionWrapper convention, Convention convData, IDocumentSession session)
        {
            convData.TicketIds = new List<string>();
            foreach (var ticket in convention.NameAndTickets)
            {
                session.Store(ticket.Value);
                convData.TicketIds.Add(ticket.Value.Id);
            }
        }

        private string ParseRoleFromType(TicketType type, string conId)
        {
            switch (type)
            {
                case TicketType.GameMasterTicket:
                    return Roles.GameMasterForConvention(conId);
                case TicketType.VolunteerTicket:
                    return Roles.VolunteerForConvention(conId);

            }

            return string.Empty;
        }


        private static void StoreConvHalls(ConventionWrapper convention, Convention convData, IDocumentSession session)
        {
            convData.HallIds = new List<string>();
            foreach (var hall in convention.NameAndHall)
            {
                session.Store(hall.Value);
                convData.HallIds.Add(hall.Value.Id);
            }
        }

        private static void StoreConvDays(ConventionWrapper convention, Convention convData, IDocumentSession session)
        {
            convData.DayIds = new List<string>();
            foreach (var day in convention.Days)
            {
                session.Store(day.Value);
                convData.DayIds.Add(day.Value.Id);
            }
        }
    }
}

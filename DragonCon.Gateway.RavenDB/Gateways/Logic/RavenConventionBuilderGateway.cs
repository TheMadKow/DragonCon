using System.Collections.Generic;
using System.Linq;
using DragonCon.Logical.Gateways;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.HallsTables;
using DragonCon.Modeling.Models.Tickets;
using Raven.Client.Documents.Session;

namespace DragonCon.RavenDB.Gateways.Logic
{
    public class RavenConventionBuilderGateway : IConventionBuilderGateway
    {
        private StoreHolder _holder;

        public RavenConventionBuilderGateway(StoreHolder holder)
        {
            _holder = holder;
        }

        public ConventionWrapper GetConventionWrapper(string id)
        {
            if (id.StartsWith("conventions/") == false)
                id = "conventions/" + id;

            using (var session = _holder.Store.OpenSession())
            {
                Convention convention = session
                    .Include<Convention>(x => x.DayIds)
                    .Include<Convention>(x => x.HallIds)
                    .Include<Convention>(x => x.TicketIds)
                    .Load<Convention>(id);

                return new ConventionWrapper
                {
                    Name = convention.Name,
                    Id = convention.Id,
                    Days = session.Load<ConDay>(convention.DayIds).Select(x => x.Value).Select(x => new ConDayWrapper(x)).ToList(),
                    Halls = session.Load<Hall>(convention.HallIds).Select(x => x.Value).ToList(),
                    Tickets = session.Load<Ticket>(convention.TicketIds).Select(x => new TicketWrapper(x.Value)).ToList(),
                };
            }
        }

        public virtual void StoreConvention(ConventionWrapper convention, IList<string> deletedIds)
        {
            using (var session = _holder.Store.OpenSession())
            {
                var conventionData = session.Load<Convention>(convention.Id) ?? new Convention();

                StoreGeneralInformation(convention, conventionData);
                StoreConvDays(convention, conventionData, session);
                StoreConvHalls(convention, conventionData, session);
                StoreConvTickets(convention, conventionData, session);

                foreach (var id in deletedIds)
                {
                    if (string.IsNullOrWhiteSpace(id))
                    {
                        session.Delete(id);
                    }
                }

                conventionData.Name = convention.Name;
                session.Store(conventionData);
                session.SaveChanges();
            }
        }

        private void StoreGeneralInformation(ConventionWrapper wrapperData, Convention conventionData)
        {
            conventionData.CreateTimeStamp = wrapperData.CreateTimeStamp;
            conventionData.UpdateTimeStamp = wrapperData.UpdateTimeStamp;
        }


        private void StoreConvTickets(ConventionWrapper convention, Convention convData, IDocumentSession session)
        {
            convData.TicketIds = new List<string>();
            foreach (var ticket in convention.Tickets)
            {
                session.Store(ticket.Model);
                convData.TicketIds.Add(ticket.Id);
            }
        }

        private static void StoreConvHalls(ConventionWrapper convention, Convention convData, IDocumentSession session)
        {
            convData.HallIds = new List<string>();
            foreach (var hall in convention.Halls)
            {
                session.Store(hall);
                convData.HallIds.Add(hall.Id);
            }
        }

        private static void StoreConvDays(ConventionWrapper convention, Convention convData, IDocumentSession session)
        {
            convData.DayIds = new List<string>();
            foreach (var day in convention.Days)
            {
                session.Store(day.Model);
                convData.DayIds.Add(day.Id);
            }
        }

    }
}

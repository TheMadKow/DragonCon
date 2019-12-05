using System.Collections.Generic;
using System.Linq;
using DragonCon.Logical.Gateways;
using DragonCon.Modeling.Helpers;
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
                    TagLine = convention.TagLine,
                    Location = convention.Location,
                    TimeStrategy = convention.TimeStrategy,
                    Id = convention.Id,
                    Days = session.Load<Day>(convention.DayIds).Select(x => x.Value).ToList(),
                    Halls = session.Load<Hall>(convention.HallIds).Select(x => x.Value).ToList(),
                    Tickets = session.Load<Ticket>(convention.TicketIds).Select(x => x.Value).ToList(),
                };
            }
        }

        public virtual void StoreConvention(ConventionWrapper convention, IList<string> deletedIds)
        {
            using (var session = _holder.Store.OpenSession())
            {
                var conventionData = convention.Id.IsNotEmptyString() ? 
                    session.Load<Convention>(convention.Id) : 
                    new Convention();

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

                session.Store(conventionData);
                session.SaveChanges();
            }
        }

        private void StoreGeneralInformation(ConventionWrapper wrapperData, Convention conventionData)
        {
            conventionData.Name = wrapperData.Name;
            conventionData.CreateTimeStamp = wrapperData.CreateTimeStamp;
            conventionData.UpdateTimeStamp = wrapperData.UpdateTimeStamp;
            conventionData.TimeStrategy = wrapperData.TimeStrategy;
            conventionData.Location = wrapperData.Location;
            conventionData.TagLine = wrapperData.TagLine;
        }


        private void StoreConvTickets(ConventionWrapper convention, Convention convData, IDocumentSession session)
        {
            convData.TicketIds = new List<string>();
            foreach (var ticket in convention.Tickets)
            {
                session.Store(ticket);
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
                session.Store(day);
                convData.DayIds.Add(day.Id);
            }
        }

    }
}

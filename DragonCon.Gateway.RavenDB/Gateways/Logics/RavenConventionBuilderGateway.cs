using System.Collections.Generic;
using System.Linq;
using DragonCon.Logical.Gateways;
using DragonCon.Modeling.Helpers;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.HallsTables;
using DragonCon.Modeling.Models.Tickets;
using DragonCon.RavenDB.Factories;
using NodaTime;
using Raven.Client.Documents.Session;

namespace DragonCon.RavenDB.Gateways.Logics
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


            using var session = _holder.Store.OpenSession();
            var convention = session
                .Include<Convention>(x => x.DayIds)
                .Include<Convention>(x => x.HallIds)
                .Include<Convention>(x => x.TicketIds)
                .Load<Convention>(id);

            var wrapperFactory = new WrapperFactory(session);
            return wrapperFactory.Wrap(convention);
        }

        public virtual void StoreConvention(ConventionWrapper convention, IList<string> deletedIds)
        {
            using (var session = _holder.Store.OpenSession())
            {
                var model = convention.Inner;
                if (model.CreateTimeStamp == Instant.MinValue)
                    model.CreateTimeStamp = SystemClock.Instance.GetCurrentInstant();

                model.UpdateTimeStamp = SystemClock.Instance.GetCurrentInstant();
                var conventionData = model.Id.IsNotEmptyString() ? 
                    session.Load<Convention>(model.Id) : 
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
            conventionData.Name = wrapperData.Inner.Name;
            conventionData.CreateTimeStamp = wrapperData.Inner.CreateTimeStamp;
            conventionData.UpdateTimeStamp = wrapperData.Inner.UpdateTimeStamp;
            conventionData.TimeStrategy = wrapperData.Inner.TimeStrategy;
            conventionData.Location = wrapperData.Inner.Location;
            conventionData.TagLine = wrapperData.Inner.TagLine;
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

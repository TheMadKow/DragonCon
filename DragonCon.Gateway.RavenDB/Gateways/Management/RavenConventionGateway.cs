using System.Collections.Generic;
using System.Linq;
using DragonCon.Features.Management.Convention;
using DragonCon.Features.Shared;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.HallsTables;
using DragonCon.Modeling.Models.System;
using DragonCon.Modeling.Models.Tickets;
using NodaTime;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace DragonCon.RavenDB.Gateways.Management
{
    public class RavenConventionGateway : IConventionGateway
    {
        private readonly StoreHolder _holder;

        public RavenConventionGateway()
        {

        }

        public RavenConventionGateway(StoreHolder holder)
        {
            _holder = holder;
        }


        public ConventionManagementViewModel BuildConventionList(IDisplayPagination pagination)
        {
            var result = new ConventionManagementViewModel();
            using (var session = _holder.Store.OpenSession())
            {
                var conventions = session
                    .Query<Convention>()
                    .Include(x => x.DayIds)
                    .Include(x => x.HallIds)
                    .Include(x => x.TicketIds)
                    .Statistics(out var stats)
                    .OrderByDescending(x => x.CreateTimeStamp)
                    .Skip(pagination.SkipCount)
                    .Take(pagination.ResultsPerPage)
                    .ToList();
                
                result.Conventions = conventions.Select(y => new ConventionWrapper(y)
                {
                    Name = y.Name,
                    Id = y.Id,
                    Days = y.DayIds == null ? 
                        new Dictionary<LocalDate, ConDayWrapper>() : 
                        session.Load<ConDay>(y.DayIds)?.Select(x => x.Value).ToDictionary(x => x.Date, x => new ConDayWrapper(x)),
                    NameAndHall = y.HallIds == null ?
                        new Dictionary<string, HallWrapper>() : 
                        session.Load<Hall>(y.HallIds)?.Select(x => x.Value).ToDictionary(x => x.Name, x => new HallWrapper(x)),
                    NameAndTickets = y.TicketIds == null ? 
                        new Dictionary<string, TicketWrapper>() :
                        session.Load<Ticket>(y.TicketIds)?.Select(x => x.Value).ToDictionary(x => x.Name, x => new TicketWrapper(x)),
                }).ToList();

                result.Configuration = session.Load<SystemConfiguration>(SystemConfiguration.Id) ?? new SystemConfiguration();
                result.Pagination = DisplayPagination.BuildForView(stats.TotalResults, pagination.SkipCount, pagination.ResultsPerPage);
            }
            return result;
        }

    }
}

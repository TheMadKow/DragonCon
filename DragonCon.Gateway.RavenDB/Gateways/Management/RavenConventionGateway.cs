using System.Collections.Generic;
using System.Linq;
using DragonCon.Features.Management.Convention;
using DragonCon.Features.Shared;
using DragonCon.Modeling.Helpers;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.HallsTables;
using DragonCon.Modeling.Models.System;
using DragonCon.Modeling.Models.Tickets;
using NodaTime;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace DragonCon.RavenDB.Gateways.Management
{
    public class RavenConventionGateway : RavenGateway, IConventionGateway
    {
        private readonly StoreHolder _holder;

        public RavenConventionGateway() : base()
        {

        }

        public RavenConventionGateway(StoreHolder holder) : base(holder)
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

                result.Pagination = DisplayPagination.BuildForView(stats.TotalResults, pagination.SkipCount, pagination.ResultsPerPage);
            }

            result.Configuration = LoadSystemConfiguration();
            return result;
        }

        public ConventionUpdateViewModel BuildConventionUpdate(string conId)
        {
            conId = conId.FixRavenId("Conventions");
            var result = new ConventionUpdateViewModel();

            using (var session = _holder.Store.OpenSession())
            {
                var convention = session
                    .Include<Convention>(x => x.DayIds)
                    .Include<Convention>(x => x.HallIds)
                    .Include<Convention>(x => x.TicketIds)
                    .Load<Convention>(conId);

                var days = session.Load<ConDay>(convention.DayIds);
                var halls = session.Load<Hall>(convention.HallIds);
                var tickets = session.Load<Ticket>(convention.TicketIds);

                result.NameDate = new NameDatesCreateUpdateViewModel
                {
                    Name = convention.Name,
                    Id = convention.Id,
                    Days = days.Select(x => new DaysViewModel(new ConDayWrapper(x.Value))).ToList()
                };

                result.Halls = new HallsUpdateViewModel
                {
                    ConventionId = convention.Id,
                    Halls = halls.Select(x => new HallWrapper(x.Value)).ToList()
                };

                result.Tickets = new TicketsUpdateViewModel()
                {
                    ConventionId = convention.Id,
                    Tickets = tickets.Select(x => new TicketViewModel(new TicketWrapper(x.Value))).ToList(),
                    AvailableDays = result.NameDate.Days
                };

                result.Details = new DetailsUpdateViewModel
                {
                    ConventionId = convention.Id,
                    Metadata = convention.Metadata,
                    Phonebook = convention.PhoneBook
                };
            }

            return result;
        }


        public void SaveSystemConfiguration(SystemConfiguration config)
        {
            using (var session = _holder.Store.OpenSession())
            {
                session.Store(config, SystemConfiguration.Id);
                session.SaveChanges();
            }
        }

    }
}

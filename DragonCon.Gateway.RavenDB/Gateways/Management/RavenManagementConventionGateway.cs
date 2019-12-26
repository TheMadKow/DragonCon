using System;
using System.Collections.Generic;
using System.Linq;
using DragonCon.Features.Management.Convention;
using DragonCon.Features.Shared;
using DragonCon.Modeling.Helpers;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.HallsTables;
using DragonCon.Modeling.Models.System;
using DragonCon.Modeling.Models.Tickets;
using Raven.Client.Documents;

namespace DragonCon.RavenDB.Gateways.Management
{
    public class RavenManagementConventionGateway : RavenGateway, IManagementConventionGateway
    {
        public RavenManagementConventionGateway(IServiceProvider provider) 
            : base(provider) { }


        public ConventionManagementViewModel BuildConventionList(IDisplayPagination pagination)
        {
            var result = new ConventionManagementViewModel();
            var conventions = Session
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
                Halls = y.HallIds == null
                    ? new List<Hall>()
                    : Session.Load<Hall>(y.HallIds)?.Select(x => x.Value).ToList(),
                Tickets = y.TicketIds == null
                    ? new List<Ticket>()
                    : Session.Load<Ticket>(y.TicketIds)?.Select(x => x.Value).ToList(),
                Days = y.DayIds == null
                    ? new List<Day>()
                    : Session.Load<Day>(y.DayIds)?.Select(x => x.Value).ToList()
            }).ToList();

            result.Pagination = DisplayPagination.BuildForView(stats.TotalResults, pagination.SkipCount, pagination.ResultsPerPage);
            return result;
        }

        public ConventionUpdateViewModel BuildConventionUpdate(string conId)
        {
            conId = conId.FixRavenId("Conventions");
            var result = new ConventionUpdateViewModel();

            var convention = Session
                .Include<Convention>(x => x.DayIds)
                .Include<Convention>(x => x.HallIds)
                .Include<Convention>(x => x.TicketIds)
                .Load<Convention>(conId);

            result.Settings = new SettingsUpdateViewModel(conId, convention.Settings);

            var days = Session.Load<Day>(convention.DayIds);
            var halls = Session.Load<Hall>(convention.HallIds);
            var tickets = Session.Load<Ticket>(convention.TicketIds);

            result.NameDate = new NameDatesCreateUpdateViewModel
            {
                Name = convention.Name,
                Id = convention.Id,
                Location = convention.Location,
                TagLine = convention.TagLine,
                TimeStrategy = convention.TimeStrategy,
                Days = days.Select(x => new DaysViewModel(x.Value)).ToList()
            };
            if (result.NameDate.Days.Any() == false)
            {
                result.NameDate.Days.Add(new DaysViewModel());
            }

            result.Halls = new HallsUpdateViewModel
            {
                ConventionId = convention.Id,
                Halls = halls.Select(x => new HallViewModel(x.Value)).ToList()
            };
            if (result.Halls.Halls.Any() == false)
            {
                result.Halls.Halls.Add(new HallViewModel());
            }

            result.Tickets = new TicketsUpdateViewModel()
            {
                ConventionId = convention.Id,
                Tickets = tickets.Select(x => new TicketViewModel(x.Value)).ToList(),
                AvailableDays = result.NameDate.Days
            };
            if (result.Tickets.Tickets.Any() == false)
            {
                result.Tickets.Tickets.Add(new TicketViewModel());
            }

            result.Details = new DetailsUpdateViewModel
            {
                ConventionId = convention.Id,
                Metadata = convention.Metadata,
                Phonebook = convention.PhoneBook
            };
            
            return result;
        }

        private SystemConfiguration LoadOrCreateConfiguration()
        {
            var config = Session.Load<SystemConfiguration>(SystemConfiguration.Id);
            if (config == null)
                config = new SystemConfiguration();

            return config;
        }

        public void SetAsManaged(string id)
        {
            var config = LoadOrCreateConfiguration();
            config.ManagedConventionId = id;
            Session.Store(config, SystemConfiguration.Id);
            Session.SaveChanges();
        }

        public void SetAsDisplay(string id)
        {
            var config = LoadOrCreateConfiguration();
            config.DisplayConventionId = id;
            Session.Store(config, SystemConfiguration.Id);
            Session.SaveChanges();
        }

        public void UpdateSettings(string convetionId, ConventionSettings settings)
        {
            var convention = Session.Load<Convention>(convetionId);
            convention.Settings = settings;
            Session.Store(convention);
            Session.SaveChanges();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DragonCon.Features.Convention.Events;
using DragonCon.Features.Convention.Shared;
using DragonCon.Features.Shared;
using DragonCon.Modeling.Helpers;
using DragonCon.Modeling.Models.Events;
using DragonCon.RavenDB.Factories;
using DragonCon.RavenDB.Index;
using Raven.Client.Documents;

namespace DragonCon.RavenDB.Gateways.Conventions
{
    public class RavenDisplayEventsGateway : RavenGateway, IDisplayEventsGateway
    {
        public RavenDisplayEventsGateway(IServiceProvider provider) : base(provider)
        {
        }

        public DisplayEventsViewModel BuildEvents(IDisplayPagination pagination, DisplayEventsViewModel.Filters filters = null)
        {
            var events = Session.Query<Events_BySeatsAgeAndEndTime.Result, Events_BySeatsAgeAndEndTime>()
                .Include(x => x.DayId)
                .Include(x => x.EventId)
                .Where(x => x.Status == EventStatus.Approved)
                .AsQueryable();
            if (filters != null)
            {
                if (filters.HideCompleted)
                {
                    // TODO
                }

                if (filters.HideTaken)
                {
                    events = events.Where(x => x.SeatsAvailable > 1);
                }

                if (filters.StartTime.IsNotEmptyString())
                {
                    // TODO
                }

                if (filters.ActivitySelection.IsNotEmptyString())
                {
                    // TODO
                }
            }

            events = events.Skip(pagination.SkipCount).Take(pagination.ResultsPerPage);
            var result = events.ToList();

            var realEvents = Session
                .Include<Event>(x => x.ConventionId)
                .Include<Event>(x => x.ActivityId)
                .Include<Event>(x => x.AgeId)
                .Include<Event>(x => x.GameHostIds)
                .Include<Event>(x => x.SubActivityId)
                .Include<Event>(x => x.HallId)
                .Load<Event>(result.Select(x => x.EventId))
                .Where(x => x.Value != null)
                .Select(x => x.Value)
                .ToList();

            var wrappers = new WrapperFactory(Session).Wrap(realEvents);
            var stats = events.ToDictionary(x => x.EventId, x => x);

            var viewModel = new DisplayEventsViewModel();
            viewModel.Pagination =
                DisplayPagination.BuildForView(result.Count, pagination.SkipCount, pagination.ResultsPerPage);
            foreach (var eventWrapper in wrappers)
            {
                var seats = stats[eventWrapper.Inner.Id];
                viewModel.Events.Add(new DisplayEventViewModel(eventWrapper, seats.SeatsCapacity, seats.SeatsTaken));
            }

            return viewModel;
        }
    }
}

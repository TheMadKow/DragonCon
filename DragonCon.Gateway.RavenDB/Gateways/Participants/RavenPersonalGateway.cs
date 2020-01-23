using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DragonCon.Features.Participant.Account;
using DragonCon.Features.Participant.Personal;
using DragonCon.Features.Shared;
using DragonCon.Logical;
using DragonCon.Logical.Identities;
using DragonCon.Modeling.Helpers;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.Events;
using DragonCon.Modeling.Models.Identities;
using DragonCon.RavenDB.Factories;
using DragonCon.RavenDB.Index;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Raven.Client.Documents;

namespace DragonCon.RavenDB.Gateways.Participants
{
    public class RavenPersonalGateway : RavenGateway, IPersonalGateway
    {
        private IIdentityFacade _facade;

        public RavenPersonalGateway(IServiceProvider provider) : base(provider)
        {
            _facade = provider.GetRequiredService<IIdentityFacade>();
        }

        public Answer AddSuggestedEvent(SuggestEventViewModel viewmodel)
        {
            try
            {
                var conEvent = new Event
                {
                    Status = EventStatus.Pending,
                    Name = viewmodel.Name,
                    Description = viewmodel.Description,
                    SpecialRequests = viewmodel.Requests ?? string.Empty,
                    ConventionDayId = viewmodel.ConventionDayId,
                    ConventionId = Actor.DisplayConvention.ConventionId,
                    AgeId = viewmodel.AgeRestrictionId,
                    ActivityId = viewmodel.ActivityId,
                    SubActivityId = viewmodel.SubActivityId,
                    Size = new SizeRestriction()
                    {
                        Max = viewmodel.Max,
                        Min = viewmodel.Min
                    },
                    GameHostIds = new List<string> { Actor.Me.Id },
                    HallId = string.Empty,
                    HallTable = null,
                };

                var timeSplit = viewmodel.StartTime.Split(':');
                var hours = int.Parse(timeSplit[0]);
                var minutes = int.Parse(timeSplit[1]);
                var num = (int)viewmodel.Duration / 1;
                var denum = (int)viewmodel.Duration % 1;

                var startTime = new LocalTime(hours, minutes);
                var endTime = startTime.PlusHours(num).PlusMinutes(denum);

                var tempTimeSlot = new TimeSlot
                {
                    From = startTime,
                    To = endTime,
                    Duration = viewmodel.Duration
                };
                conEvent.TimeSlot = tempTimeSlot;

                Session.Store(conEvent);

                var engagement = Session.Query<UserEngagement>().FirstOrDefault(x =>
                    x.ConventionId == Actor.DisplayConvention.ConventionId &&
                    x.ParticipantId == Actor.Me.Id);

                if (engagement == null)
                {
                    engagement = new UserEngagement
                    {
                        CreatorId = Actor.Me.Id,
                        ParticipantId = Actor.Me.Id,
                        ConventionId = Actor.DisplayConvention.ConventionId,
                        ConventionStartDate = Actor.DisplayConvention.Days
                            .Min(x => x.Date)
                            .ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),

                        IsLongTerm = true,
                    };
                    Session.Store(engagement);
                }

                engagement.SuggestedEventIds.Add(conEvent.Id);
                Session.Store(engagement);

                Session.SaveChanges();
                return Answer.Success;
            }
            catch (Exception e)
            {
                return Answer.Error(e.Message);
            }
        }

        public PersonalViewModel BuildPersonalViewModel()
        {
            var currentConvention = Actor.DisplayConvention.ConventionId;
            var currentUser = Actor.Me.Id;

            var myEngagement = Session.Query<UserEngagement>()
                .Include(x => x.ConventionId)
                .Include(x => x.ParticipantId)
                .Include(x => x.EventIds)
                .Include(x => x.SuggestedEventIds)
                .FirstOrDefault(x => x.ConventionId == currentConvention && x.ParticipantId == currentUser);

            var myRelatedEngagements = Session.Query<UserEngagement>()
                .Include(x => x.ParticipantId)
                .Include(x => x.EventIds)
                .Where(x => x.CreatorId == currentUser
                            && x.ParticipantId != currentUser
                            && x.ConventionId == currentConvention)
                .ToList();

            if (myEngagement == null)
            {
                myEngagement = new UserEngagement
                {
                    CreatorId = currentUser,
                    ParticipantId = currentUser,
                    ConventionId = currentConvention,
                    ConventionStartDate = Actor.DisplayConvention.Days
                        .Min(x => x.Date)
                        .ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),

                    IsLongTerm = true,
                };
                Session.Store(myEngagement);
                Session.SaveChanges();
            }

            var allEvents = new List<string>(myEngagement.EventIds);
            allEvents.AddRange(myEngagement.SuggestedEventIds);

            var allParticipants = new List<string>();
            foreach (var myRelatedEngagement in myRelatedEngagements)
            {
                allParticipants.Add(myRelatedEngagement.ParticipantId);
                allEvents.AddRange(myRelatedEngagement.EventIds);
            }

            // Preload Events
            Session
                .Include<Event>(x => x.ConventionDayId)
                .Load<Event>(allEvents);

            var result = new PersonalViewModel();
            var wrapperFactory = new WrapperFactory(Session);
            result.MyEngagement = wrapperFactory.Wrap(myEngagement);
            result.RelatedEngagements = wrapperFactory.Wrap(myRelatedEngagements);
            return result;
        }

        public LongTermParticipant GetParticipant(string id)
        {
            return Session.Load<LongTermParticipant>(id);
        }

        public async Task<Answer> ChangePassword(PasswordChangeViewModel viewmodel)
        {
            var user = await _facade.GetParticipantByIdAsync(Actor.Me.Id);
            var result = await _facade.ChangePasswordAsync(user, viewmodel.OldPassword, viewmodel.newPassword);
            return result.IsSuccess ? Answer.Success : Answer.Error(result.Details);
        }

        public async Task<Answer> UpdateDetails(DetailsUpdateViewModel viewModel)
        {
            var user = await _facade.GetParticipantByIdAsync(Actor.Me.Id);
            user.YearOfBirth = viewModel.YearOfBirth;
            user.FullName = viewModel.FullName;
            user.PhoneNumber = viewModel.PhoneNumber;
            user.IsAllowingPromotions = viewModel.IsAllowingPromotions;
            user.Email = viewModel.Email;

            var result = await _facade.UpdateParticipant(user);
            var message = "";

            if (result.IsSuccess && result.IsEmailChange)
            {
                await _facade.LogoutAsync();
                message = "Logout";
            }

            return result.IsSuccess ?
                new Answer(AnswerType.Success)
                {
                    Message = message
                }
                : Answer.Error(result.Details);
        }

        public DisplaySelectableEventsViewModel BuildEvents(string forUserId, IDisplayPagination pagination, DisplayEventsViewModel.Filters filters)
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

            var viewModel = new DisplaySelectableEventsViewModel();
            viewModel.Pagination =
                DisplayPagination.BuildForView(result.Count, pagination.SkipCount, pagination.ResultsPerPage);
            foreach (var eventWrapper in wrappers)
            {
                var seats = stats[eventWrapper.Inner.Id];
                viewModel.Events.Add(new DisplayEventViewModel(eventWrapper, seats.SeatsCapacity, seats.SeatsTaken));
            }

            var engagement = Session.Query<UserEngagement>().FirstOrDefault(x =>
                x.ConventionId == Actor.DisplayConvention.ConventionId &&
                x.ParticipantId == Actor.Me.Id);

            viewModel.EventIdsSelected = engagement.EventIds;
            return viewModel;
        }
    }
}
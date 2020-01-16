using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DragonCon.Features.Convention.Home;
using DragonCon.Features.Participant.Account;
using DragonCon.Features.Participant.Personal;
using DragonCon.Logical;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.Events;
using DragonCon.Modeling.Models.Identities;
using DragonCon.RavenDB.Factories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.Extensions.DependencyInjection;
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
            throw new NotImplementedException();
            //try
            //{
            //    var lazyAge = Session.Advanced.Lazily.Load<AgeGroup>(viewmodel.AgeRestrictionId);
            //    var conEvent = new Event()
            //    {

            //        Name = viewmodel.Name,
            //        Description = viewmodel.Description,
            //        SpecialRequests = viewmodel.Requests,
            //        ActivityId = viewmodel.ActivityId,
            //        //TimeSlot = new TimeSlot
            //        //{
            //        //    From = viewmodel.StartTime,
            //        //    To = viewmodel.StartTime.Plus(viewmodel.Period)
            //        //},
            //        //Size = viewmodel.SizeRestrictions,
            //        GameMasterIds = new List<string> { Actor.Me.Id },
            //        Status = EventStatus.Pending,
            //        HallId = string.Empty,
            //        HallTable = null,
            //    };

            //    Session.Advanced.Eagerly.ExecuteAllPendingLazyOperations();
            //    conEvent.AgeId = lazyAge.Value.Id;

            //    Session.Store(conEvent);
            //    Session.SaveChanges();

            //    return new Answer(AnswerType.Success);

            //}
            //catch (Exception e)
            //{
            //    return new Answer(AnswerType.Error)
            //    {
            //        InternalException = e
            //    };
            //    // TODO maybe log
            //}
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

            var allEvents = myEngagement.EventIds;
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
            var user = await _facade.GetUserByUserIdAsync(Actor.Me.Id);
            var result = await _facade.ChangePasswordAsync(user, viewmodel.OldPassword, viewmodel.newPassword);
            return result.IsSuccess ? Answer.Success : Answer.Error(result.Errors.FirstOrDefault());
        }

        public async Task<Answer> UpdateDetails(DetailsUpdateViewModel viewModel)
        {
            var user = await _facade.GetUserByUserIdAsync(Actor.Me.Id);
            user.YearOfBirth = viewModel.YearOfBirth;
            user.FullName = viewModel.FullName;
            user.IsAllowingPromotions = viewModel.IsAllowingPromotions;
            bool emailReplace = user.Email.ToLower() != viewModel.Email.ToLower();
            user.Email = viewModel.Email;

            var result = await _facade.UpdateParticipant(user);
            var message = "";
            if (result.IsSuccess && emailReplace)
            {
                await _facade.LogoutAsync(user.UserName);
                message = "Logout";
            }

            return result.IsSuccess ?
                new Answer(AnswerType.Success)
                {
                    Message = message
                }
                : Answer.Error(result.Errors.FirstOrDefault());
        }
    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DragonCon.Features.Convention.Home;
using DragonCon.Features.Participant.Personal;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.Events;
using DragonCon.Modeling.Models.Identities;
using DragonCon.Modeling.ViewModels;
using DragonCon.RavenDB.Factories;
using Raven.Client.Documents;

namespace DragonCon.RavenDB.Gateways.Participants
{
    public class RavenPersonalGateway : RavenGateway, IPersonalGateway
    {
        public RavenPersonalGateway(IServiceProvider provider) : base(provider)
        {
        }

        public Answer AddSuggestedEvent(SuggestEventViewModel viewmodel)
        {
            try
            {
                    var lazyAge = Session.Advanced.Lazily.Load<AgeGroup>(viewmodel.AgeRestrictionId);
                    var conEvent = new Event()
                    {
                        Name = viewmodel.Name,
                        ConventionDayId = viewmodel.DayId,
                        Description = viewmodel.Description,
                        SpecialRequests = viewmodel.Requests,
                        ActivityId = viewmodel.ActivityId,
                        SubActivityId = viewmodel.SystemId,
                        TimeSlot = new TimeSlot
                        {
                            From = viewmodel.StartTime,
                            To = viewmodel.StartTime.Plus(viewmodel.Period)
                        },
                        Size = viewmodel.SizeRestrictions,
                        GameMasterIds = new List<string> {viewmodel.CreatorId},
                        Status = EventStatus.Pending,
                        Tags = viewmodel.Tags,
                        HallId = string.Empty,
                        HallTable = null,
                    };

                    Session.Advanced.Eagerly.ExecuteAllPendingLazyOperations();
                    conEvent.AgeId = lazyAge.Value.Id;

                    Session.Store(conEvent);
                    Session.SaveChanges();

                    return new Answer(AnswerType.Success);
                
            }
            catch (Exception e)
            {
                return new Answer(AnswerType.Error)
                {
                    InternalException = e
                };
                // TODO maybe log
            }
        }

        public PersonalViewModel BuildPersonalViewModel()
        {
            var currentConvention = Actor.DisplayConvention.ConventionId;
            var currentUser = Actor.Me.Id;

            var myEngagement = Session.Query<ConventionEngagement>()
                .Include(x => x.EventIds)
                .Include(x => x.SuggestedEventIds)
                .FirstOrDefault(x => x.ConventionId == currentConvention && x.ParticipantId == currentUser);
        
            var myRelatedEngagements = Session.Query<ConventionEngagement>()
                .Include(x => x.ParticipantId)
                .Include(x => x.EventIds)
                .Where(x => x.CreatorId == currentUser)
                .ToList();

            if (myEngagement == null)
            {
                myEngagement = new ConventionEngagement
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

            allEvents = allEvents.Distinct().ToList();
            allParticipants = allParticipants.Distinct().ToList();
            
            var result = new PersonalViewModel();
            var wrapperFactory = new WrapperFactory(Session);
            result.MyEngagement = wrapperFactory.Wrap(myEngagement);
            result.RelatedEngagements = wrapperFactory.Wrap(myRelatedEngagements);
            return result;
        }
    }
}
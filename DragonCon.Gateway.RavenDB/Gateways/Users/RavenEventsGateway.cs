using System;
using System.Collections.Generic;
using DragonCon.Features.Home;
using DragonCon.Features.Home.Convention;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Events;
using DragonCon.Modeling.Models.Identities;
using DragonCon.Modeling.ViewModels;
using SystemClock = NodaTime.SystemClock;

namespace DragonCon.RavenDB.Gateways.Users
{
    public class RavenEventsGateway : RavenGateway, IEventsGateway
    {
        public RavenEventsGateway(StoreHolder holder, IActor actor) : base(holder, actor)
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
                        ParticipantIds = new List<string>(),
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
    }
}
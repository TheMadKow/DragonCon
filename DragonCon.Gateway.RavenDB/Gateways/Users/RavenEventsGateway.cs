using System;
using System.Collections.Generic;
using DragonCon.Features.Home;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Events;
using DragonCon.Modeling.ViewModels;
using SystemClock = NodaTime.SystemClock;

namespace DragonCon.RavenDB.Gateways.Users
{
    public class RavenEventsGateway : RavenGateway, IEventsGateway
    {
        public RavenEventsGateway(StoreHolder holder) :
            base(holder)
        {
        }

        public Answer AddSuggestedEvent(SuggestEventViewModel viewmodel)
        {
            try
            {

                using (var session = OpenSession)
                {
                    var lazyAge = session.Advanced.Lazily.Load<AgeRestrictionTemplate>(viewmodel.AgeRestrictionId);
                    var conEvent = new ConEvent()
                    {
                        Name = viewmodel.Name,
                        ConventionDayId = viewmodel.DayId,
                        Description = viewmodel.Description,
                        SpecialRequests = viewmodel.Requests,
                        ActivityId = viewmodel.ActivityId,
                        SystemId = viewmodel.SystemId,
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
                        Table = null,
                    };

                    session.Advanced.Eagerly.ExecuteAllPendingLazyOperations();
                    conEvent.AgeId = lazyAge.Value.Id;

                    session.Store(conEvent);
                    session.SaveChanges();

                    return new Answer(AnswerType.Success);
                }
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
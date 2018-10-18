using System;
using System.Collections.Generic;
using System.Text;
using DragonCon.Logical.Gateways.Users;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Events;
using DragonCon.Modeling.ViewModels;
using Microsoft.AspNetCore.Authentication;
using SystemClock = NodaTime.SystemClock;

namespace DragonCon.Gateway.RavenDB.Gateways.Users
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

            using (var session = Session)
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
                    GameMasterId = viewmodel.CreatorId,
                    HelperIds = viewmodel.HelperIds,
                    ParticipantIds = new List<string>(),
                    HasBeenRevised = false,
                    Status = EventStatus.Pending,
                    Tags = viewmodel.Tags,
                    TableId = string.Empty,
                    Changes = new List<string>()
                };

                session.Advanced.Eagerly.ExecuteAllPendingLazyOperations();
                conEvent.Age = lazyAge.Value;
                
                var create = new EventChange
                {
                    ExecutorId = viewmodel.CreatorId,
                    IsCreationChange = true,
                    Changes = new List<EventChange.FieldChange>()
                };
                session.Store(create);

                conEvent.Changes.Add(create.Id);
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

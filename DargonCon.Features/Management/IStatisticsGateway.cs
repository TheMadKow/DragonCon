using System.Collections.Generic;
using DragonCon.Features.Shared;
using DragonCon.Modeling.Models.Events;

namespace DragonCon.Features.Management
{
    public interface IStatisticsGateway : IGateway
    {
        EventWrapper GetEvent(string id);
        void StoreEvent(EventWrapper @event);
        List<EventWrapper> GetAllEvents(string conventionId);
    }
}

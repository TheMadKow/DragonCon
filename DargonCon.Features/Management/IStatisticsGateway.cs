using System.Collections.Generic;
using DragonCon.Features.Shared;
using DragonCon.Modeling.Models.Events;

namespace DragonCon.Features.Management
{
    public interface IStatisticsGateway : IGateway
    {
        ConEventWrapper GetEvent(string id);
        void StoreEvent(ConEventWrapper conEvent);
        List<ConEventWrapper> GetAllEvents(string conventionId);
    }
}

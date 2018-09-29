using System.Collections.Generic;
using DragonCon.Modeling.Models.Events;

namespace DragonCon.Modeling.Gateways
{
    public interface IEventsGateway
    {
        ConEventWrapper GetEvent(string id);
        void StoreEvent(ConEventWrapper id);
        List<ConEventWrapper> GetAllEvents(string conventionId);
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using DragonCon.Modeling.Models.Events;

namespace DragonCon.Logical.Gateways.Management
{
    public interface IStatisticsGateway
    {
        ConEventWrapper GetEvent(string id);
        void StoreEvent(ConEventWrapper conEvent);
        List<ConEventWrapper> GetAllEvents(string conventionId);

    }
}

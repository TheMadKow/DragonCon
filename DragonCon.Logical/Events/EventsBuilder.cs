using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragonCon.Modeling.Gateways;

namespace DragonCon.Logical.Events
{
    public class EventsBuilder
    {
        private IEventsGateway _gateway;

        public EventsBuilder(IEventsGateway gateway)
        {
            _gateway = gateway;
        }

    }
}

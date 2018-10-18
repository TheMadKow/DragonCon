using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragonCon.Logical.Gateways.Management;

namespace DragonCon.Logical.Events
{
    public class EventsBuilder
    {
        private IContentGateway _gateway;


        public EventsBuilder(IContentGateway gateway)
        {
            _gateway = gateway;
        }

    }
}

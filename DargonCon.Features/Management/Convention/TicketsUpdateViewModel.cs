using System.Collections.Generic;
using DragonCon.Modeling.Models.Tickets;
using NodaTime;

namespace DragonCon.Features.Management.Convention
{
    public class TicketsUpdateViewModel
    {
        public List<TicketWrapper> Tickets { get; set; }
    }
}
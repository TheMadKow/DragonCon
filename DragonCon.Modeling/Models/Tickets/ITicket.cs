using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonCon.Modeling.Models.Tickets
{
    public interface ITicket
    {
        string Id { get; set; }
        List<string> DayIds { get; set; }
        string Name { get; set; }
        double Price { get; set; }
        string TransactionCode { get; set; }

        int? ActivitiesAllowed { get; set; }
        TicketLimitation TicketLimitation { get; set; }
    }
}

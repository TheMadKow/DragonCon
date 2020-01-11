using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonCon.Modeling.Models.Tickets
{
    public enum TicketType
    {
        [Description("רגיל")]
        NotLimited,
        [Description("צוות")]
        Staff,
        [Description("הנחיה")]
        GameMaster,
        [Description("התנדבות")]
        Volunteer
    }
}

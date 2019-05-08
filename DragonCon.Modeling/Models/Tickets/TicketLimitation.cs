using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonCon.Modeling.Models.Tickets
{
    public enum TicketLimitation
    {
        [Description("רגיל")]
        NotLimited,
        [Description("הנחיה")]
        GameMaster,
        [Description("סיוע הנחיה")]
        GameHelper,
        [Description("התנדבות")]
        Volunteer
    }
}

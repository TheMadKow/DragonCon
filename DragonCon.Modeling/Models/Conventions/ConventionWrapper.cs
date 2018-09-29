using System.Collections.Generic;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.HallsTables;
using DragonCon.Modeling.Models.Tickets;
using NodaTime;

namespace DragonCon.Modeling.Models.Conventions
{
    public class ConventionWrapper : Wrapper<Convention>
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public Dictionary<LocalDate, ConventionDay> Days { get; set; }
        public Dictionary<string, Hall> NameAndHall { get; set; }
        public Dictionary<string, TicketWrapper> NameAndTickets { get; set; }

    }
}

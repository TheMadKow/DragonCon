using System.Collections.Generic;
using DragonCon.Modeling.Models.Convention;
using NodaTime;

namespace DragonCon.Modeling.Models.Wrappers
{
    public class ConventionWrapper
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public Dictionary<LocalDate, ConventionDay> Days { get; set; }
        public Dictionary<string, Hall> NameAndHall { get; set; }
        public Dictionary<string, TicketWrapper> NameAndTickets { get; set; }

    }
}

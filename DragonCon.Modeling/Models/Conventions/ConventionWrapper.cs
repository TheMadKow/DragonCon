using System.Collections.Generic;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.HallsTables;
using DragonCon.Modeling.Models.Tickets;
using NodaTime;

namespace DragonCon.Modeling.Models.Conventions
{
    public class ConventionWrapper : Wrapper<Convention>
    {
        public ConventionWrapper()
        {
            Model = new Convention();
        }

        public ConventionWrapper(Convention model) : base(model) { }

        public string Id { get => Model.Id; set => Model.Id = value; }
        public string Name { get => Model.Name; set => Model.Name = value; }

        public Dictionary<LocalDate, ConDayWrapper> Days { get; set; } = new Dictionary<LocalDate, ConDayWrapper>();
        public Dictionary<string, HallWrapper> NameAndHall { get; set; } = new Dictionary<string, HallWrapper>();
        public Dictionary<string, TicketWrapper> NameAndTickets { get; set; } = new Dictionary<string, TicketWrapper>();

    }
}

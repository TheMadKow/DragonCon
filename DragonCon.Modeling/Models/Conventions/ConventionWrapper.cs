﻿using System.Collections.Generic;
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

        public List<Hall> Halls { get; set; } = new List<Hall>();
        public List<TicketWrapper> Tickets { get; set; } = new List<TicketWrapper>();
        public List<ConDayWrapper> Days { get; set; } = new List<ConDayWrapper>();
   
        public Instant CreateTimeStamp { get => Model.CreateTimeStamp; set => Model.CreateTimeStamp = value; }
        public Instant UpdateTimeStamp { get => Model.UpdateTimeStamp; set => Model.UpdateTimeStamp = value; }
    }
}

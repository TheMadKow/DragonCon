using System;
using System.Collections.Generic;
using System.Text;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.HallsTables;
using DragonCon.Modeling.Models.Tickets;

namespace DragonCon.Modeling.Models.System
{
    public class SystemState
    {
        public long BuildMilliseconds { get; set; }

        public ActiveConventionState ActiveConvention { get;set; }
        public SystemConfiguration Configurations { get; set; }

        public List<Activity> Activities { get; set; } = new List<Activity>();
        public List<AgeGroup> AgeGroups { get; set; } = new List<AgeGroup>();

        public bool HasActiveConvention => ActiveConvention != null;
        public class ActiveConventionState
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public List<Hall> Halls { get; set; } = new List<Hall>();
            public List<Ticket> Tickets { get; set; } = new List<Ticket>();
            public List<Day> Days { get; set; } = new List<Day>();

        }

    }
}

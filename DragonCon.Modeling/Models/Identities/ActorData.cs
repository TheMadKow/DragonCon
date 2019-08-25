using System.Collections.Generic;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.HallsTables;
using DragonCon.Modeling.Models.System;
using DragonCon.Modeling.Models.Tickets;

namespace DragonCon.Modeling.Models.Identities
{

    public interface IActor
    {
        Actor.ParticipantActor Participant { get; set; }
        Actor.SystemState State { get; set; }
        bool HasSystemRole(SystemRoles role);
        bool HasConventionRole(ConventionRoles role);
    }

    public class Actor : IActor
    {
        public ParticipantActor Participant { get; set; }
        public SystemState State { get; set; }

        public bool HasSystemRole(SystemRoles role) => Participant.SystemRoles.Contains(role);
        public bool HasConventionRole(ConventionRoles role) => Participant.ConventionRoles.Contains(role);

        public class ParticipantActor
        {
            public string Id { get; set; }
            public string FullName { get; set; }
            public IList<SystemRoles> SystemRoles { get; } = new List<SystemRoles>();
            public IList<ConventionRoles> ConventionRoles { get;set; } = new List<ConventionRoles>();
        }

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
}

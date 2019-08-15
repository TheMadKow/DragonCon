using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.Identities;
using Raven.Client.Documents.Operations.Replication;

namespace DragonCon.App.Helpers
{
    public interface IActor
    {
        FullParticipant Participant { get; set; }
        Actor.ConventionActor Convention { get; set; }

        bool HasSystemRole(SystemRoles role);
        bool HasConventionRole(ConventionRoles role);
    }

    public class Actor : IActor
    {
        public FullParticipant Participant { get; set; }
        public ConventionActor Convention { get; set; }

        public bool HasSystemRole(SystemRoles role) => Participant.HasRole(role);
        public bool HasConventionRole(ConventionRoles role) => Convention.ConventionRoles.Contains(role);

        public class ConventionActor
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public List<ConventionRoles> ConventionRoles { get; set; }
        }
    }
}

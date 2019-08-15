using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.Identities;
using Microsoft.AspNetCore.Http;
using NodaTime;

namespace DragonCon.Modeling.Models.Identities
{

    public interface IActor
    {
        Actor.ParticipantActor Participant { get; set; }
        Actor.ConventionActor Convention { get; set; }
        bool HasSystemRole(SystemRoles role);
        bool HasConventionRole(ConventionRoles role);
    }

    public class Actor : IActor
    {
        public ParticipantActor Participant { get; set; }
        public ConventionActor Convention { get; set; }

        public bool IsPopulated => Participant != null && Convention != null;

        public bool HasSystemRole(SystemRoles role) => Participant.HasRole(role);
        public bool HasConventionRole(ConventionRoles role) => Convention.ConventionRoles.Contains(role);


        public class ParticipantActor
        {
            public string Id { get; set; }
            public string FullName { get; set; }
            public IList<SystemRoles> RoleList { get; } = new List<SystemRoles>();
            public bool HasRole(SystemRoles role)
            {
                return RoleList.Contains(role);
            }

        }

        public class ConventionActor
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public List<ConventionRoles> ConventionRoles { get; set; }
        }
    }
}

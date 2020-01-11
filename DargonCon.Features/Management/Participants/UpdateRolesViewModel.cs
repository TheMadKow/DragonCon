using System;
using System.Collections.Generic;
using System.Text;
using DragonCon.Modeling.Models.Identities;

namespace DragonCon.Features.Management.Participants
{
    public class UpdateRolesViewModel
    {
        public string ParticipantId { get; set; } = string.Empty;
        public string ParticipantName { get; set; } = string.Empty;
        public bool IsLongTerm { get; set; } 

        public IList<SystemRoles> SystemRoles = new List<SystemRoles>();
        public IList<ConventionRoles> ConventionRoles = new List<ConventionRoles>();
    }
}

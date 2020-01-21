using System.Collections.Generic;
using System.Linq;
using DragonCon.Modeling.Helpers;
using DragonCon.Modeling.Models.Identities;
using Raven.Identity;

namespace DragonCon.RavenDB.Identities
{
    public class EmailIdentity : IdentityUser
    {
        public string LongTermId { get; set; } = string.Empty;

        public override IReadOnlyList<string> Roles => SystemRoles.Select(x => x.ToString()).ToList();

        public IList<SystemRoles> SystemRoles { get; } = new List<SystemRoles>();


        public bool HasRole(SystemRoles role)
        {
            return SystemRoles.Contains(role);
        }

        public void AddRole(SystemRoles role)
        {
            if (SystemRoles.Missing(role))
                SystemRoles.Add(role);
        }

        public void RemoveRole(SystemRoles role)
        {
            if (SystemRoles.Contains(role))
                SystemRoles.Remove(role);
        }
    }
}

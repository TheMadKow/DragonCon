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

    }
}

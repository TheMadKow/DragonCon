using System;
using System.Collections.Generic;
using System.Linq;
using DragonCon.Modeling.Helpers;
using DragonCon.Modeling.Models.Payment;
using DragonCon.Modeling.Models.Tickets;
using NodaTime;
using Raven.Identity;

namespace DragonCon.Modeling.Models.Identities
{
    public class LongTermParticipant : IdentityUser, IParticipant
    {
        public bool IsAllowingPromotions { get; set; }
 
        public Dictionary<string, PaymentInvoice> ConventionAndPayment { get;set; }

        public LocalDate DayOfBirth { get; set; }

        public string FirstName
        {
            get
            {
                try
                {
                    var breakName = FullName.Split(new []{' '}, StringSplitOptions.RemoveEmptyEntries);
                    return breakName[0].Trim();
                }
                catch
                {
                    return FullName;
                }
            }
        }

        public string FullName { get; set; }

        #region Roles 

        public IList<SystemRoles> RoleList { get; } = new List<SystemRoles>();

        public override IReadOnlyList<string> Roles => RoleList.Select(x => x.ToString()).ToList();

        public bool HasRole(SystemRoles role)
        {
            return RoleList.Contains(role);
        }

        public void AddRole(SystemRoles role)
        {
            if (RoleList.Missing(role))
                RoleList.Add(role);
        }

        public void RemoveRole(SystemRoles role)
        {
            if (RoleList.Contains(role))
                RoleList.Remove(role);
        }

        #endregion
    }
}
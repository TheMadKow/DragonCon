using System;
using System.Collections.Generic;
using System.Linq;
using DragonCon.Modeling.Helpers;
using DragonCon.Modeling.Models.Payment;
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
        // We must Override this Property.
        public override IReadOnlyList<string> Roles => new List<string>();

        public IList<SystemRoles> SystemRoles { get; } = new List<SystemRoles>();

        
        public string ActiveConventionTerm { get; set; } = string.Empty;
      
        public IList<ConventionRoles> ActiveConventionRoles { get; set; } = new List<ConventionRoles>();


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


        public bool HasRole(ConventionRoles role)
        {
            return ActiveConventionRoles.Contains(role);
        }

        public void AddRole(ConventionRoles role)
        {
            var missing = ActiveConventionRoles.Missing(role);
            if (missing)
            {
                ActiveConventionRoles.Add(role);
            }
        }

        public void RemoveRole(ConventionRoles role)
        {
            var exists = ActiveConventionRoles.Contains(role);
            if (exists)
            {
                ActiveConventionRoles.Remove(role);
            }
        }

        #endregion
    }
}
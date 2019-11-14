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
        public string ActiveConventionTerm { get; set; } = string.Empty;
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
        public IList<(string ConventionId, ConventionRoles Role)> ConventionRoles { get; } = new List<(string ConventionId, ConventionRoles Role)>();

        public IList<ConventionRoles> ActiveConventionRoles
        {
            get
            {
                return ConventionRoles
                    .Where(x => x.ConventionId == ActiveConventionTerm)
                    .Select(x => x.Role).ToList();
            }
        }


        public bool HasRole(SystemRoles role)
        {
            return SystemRoles.Contains(role);
        }
        public bool HasRole(ConventionRoles role, string conventionId)
        {
            return ConventionRoles.Any(x => x.ConventionId == conventionId && 
                                            x.Role == role);
        }

        public void AddRole(SystemRoles role)
        {
            if (SystemRoles.Missing(role))
                SystemRoles.Add(role);
        }

        public void AddRole(ConventionRoles role, string conventionId)
        {
            var exists = ConventionRoles.Any(x => x.ConventionId == conventionId && x.Role == role);
            if (exists == false)
            {
                ConventionRoles.Add((conventionId, role));
            }
        }

        public void RemoveRole(SystemRoles role)
        {
            if (SystemRoles.Contains(role))
                SystemRoles.Remove(role);
        }

        public void RemoveRole(ConventionRoles role, string conventionId)
        {
            var exists = ConventionRoles.Any(x => x.ConventionId == conventionId && x.Role == role);
            if (exists)
            {
                ConventionRoles.Remove((conventionId, role));
            }
        }

        #endregion
    }
}
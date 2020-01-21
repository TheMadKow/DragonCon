using System;
using System.Collections.Generic;

namespace DragonCon.Modeling.Models.Identities
{
    public class LongTermParticipant : IParticipant
    {
        public string Id { get; }
        public bool IsAllowingPromotions { get; set; }
        public int YearOfBirth { get; set; }
        public IList<SystemRoles> SystemRoles { get; } = new List<SystemRoles>();
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
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
    }
}
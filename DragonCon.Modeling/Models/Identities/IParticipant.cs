using System;
using System.Collections.Generic;
using NodaTime;

namespace DragonCon.Modeling.Models.Identities
{
    public interface IParticipant
    {
        
        string FirstName { get; }
        string FullName { get; set; }
        string PhoneNumber { get; set; }
        LocalDate DayOfBirth { get; set; }


        bool HasRole(ConventionRoles role);
        void AddRole(ConventionRoles role);
        void RemoveRole(ConventionRoles role);
        
        string ActiveConventionTerm { get; set; }
        IList<ConventionRoles> ActiveConventionRoles { get; }
    }
}
using System;
using System.Collections.Generic;
using NodaTime;

namespace DragonCon.Modeling.Models.Identities
{
    public interface IParticipant
    {
        
        string Id { get;  }
        string FirstName { get; }
        string FullName { get; set; }
        string PhoneNumber { get; set; }
        int YearOfBirth { get; set; }
    }
}
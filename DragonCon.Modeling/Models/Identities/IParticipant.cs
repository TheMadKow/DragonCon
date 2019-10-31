using System;
using NodaTime;

namespace DragonCon.Modeling.Models.Identities
{
    public interface IParticipant
    {
        string FirstName { get; }
        string FullName { get; set; }
        string PhoneNumber { get; set; }
        LocalDate DayOfBirth { get; set; }
    }
}
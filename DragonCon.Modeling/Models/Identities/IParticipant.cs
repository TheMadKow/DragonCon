using System;
using NodaTime;

namespace DragonCon.Modeling.Models.Identities
{
    public interface IParticipant
    {
        LocalDate DayOfBirth { get; set; }
        string Email { get; set; }
        string FirstName { get; }
        string FullName { get; set; }
        string PhoneNumber { get; set; }
    }
}
using System;

namespace DragonCon.Modeling.Models.Identities
{
    public interface IParticipant
    {
        DateTimeOffset DayOfBirth { get; set; }
        string Email { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string PhoneNumber { get; set; }
    }
}
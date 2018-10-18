using System;

namespace DragonCon.Modeling.Models.Identities
{
    public interface IParticipant
    {
        DateTimeOffset DayOfBirth { get; set; }
        string EmailAddress { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string PhoneNumber { get; set; }
    }
}
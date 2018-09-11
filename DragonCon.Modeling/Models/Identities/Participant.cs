using System;

namespace DragonCon.Modeling.Models.Identities
{
    public class Participant : IParticipant
    {
        public string Id { get; set;}
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTimeOffset DayOfBirth { get; set; }
        public string PhoneNumber { get; set; }
    }
}

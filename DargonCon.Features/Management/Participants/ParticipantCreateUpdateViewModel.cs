using NodaTime;

namespace DragonCon.Features.Management.Participants
{
    public class ParticipantCreateUpdateViewModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int YearOfBirth { get; set; }
        public bool IsAllowingPromotions { get; set; }
    }
}
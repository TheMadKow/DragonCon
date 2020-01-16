using System.ComponentModel.DataAnnotations;
using NodaTime;

namespace DragonCon.Features.Management.Participants
{
    public class ParticipantCreateUpdateViewModel
    {
        public string? Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "יש להזין שם מלא")]
        public string FullName { get; set; }
        
        public string? Email { get; set; }
        
        [Required(AllowEmptyStrings = false, ErrorMessage = "נא להזין מספר חוקי")]
        [Phone(ErrorMessage = "נא להזין מספר חוקי")]
        public string PhoneNumber { get; set; }
        
        [Range(1900, int.MaxValue, ErrorMessage = "ניתן להזין מספרים חיוביים מעל 1900")]
        public int YearOfBirth { get; set; }

        public bool? IsAllowingPromotions { get; set; }
    }
}
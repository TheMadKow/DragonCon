using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DragonCon.Features.Participant.Account
{
    public class AccountLoginViewModel
    {
        [Required(AllowEmptyStrings = false)]
        [EmailAddress(ErrorMessage = "נא להזין כתובת חוקית")]
        public string Email { get; set; }
       
        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; }

    }

    public class AccountRegisterViewModel
    {
        [Required(AllowEmptyStrings = false)]
        public string FullName { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "הסיסמאות לא תואמות")]
        public string ConfirmPassword { get; set; }

        [Required(AllowEmptyStrings = false)]
        [EmailAddress(ErrorMessage = "נא להזין כתובת חוקית")]
        public string Email { get; set; }
        [Required(AllowEmptyStrings = false)]
        [Phone(ErrorMessage = "נא להזין מספר חוקי")]
        public string PhoneNumber { get; set; }
      
        [Range(1900, int.MaxValue, ErrorMessage = "ניתן להזין מספרים חיוביים מעל 1900")]
        public int YearOfBirth { get; set; }
        public bool IsAllowingPromotions { get; set; }
    }
}

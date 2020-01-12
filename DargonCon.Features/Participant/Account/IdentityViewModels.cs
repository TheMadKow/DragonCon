using System.ComponentModel.DataAnnotations;

namespace DragonCon.Features.Participant.Account
{

    public class UpdateAccountViewModel
    {
        public DetailsUpdateViewModel Details { get; set; } = new DetailsUpdateViewModel();
        public PasswordChangeViewModel Password { get; set; } = new PasswordChangeViewModel();
    }
    public class DetailsUpdateViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "יש להזין שם מלא")]
        public string FullName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "נא להזין כתובת חוקית")]
        [EmailAddress(ErrorMessage = "נא להזין כתובת חוקית")]
        public string Email { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "נא להזין מספר חוקי")]
        [Phone(ErrorMessage = "נא להזין מספר חוקי")]
        public string PhoneNumber { get; set; }

        [Range(1900, int.MaxValue, ErrorMessage = "ניתן להזין מספרים חיוביים מעל 1900")]
        public int YearOfBirth { get; set; }
        public bool IsAllowingPromotions { get; set; }
    }

    public class PasswordChangeViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "יש להקליד סיסמה")]
        public string OldPassword { get; set; }
        
        [Required(AllowEmptyStrings = false, ErrorMessage = "יש להקליד סיסמה")]
        public string newPassword { get; set; }

        [Compare("newPassword", ErrorMessage = "הסיסמאות לא תואמות")]
        public string ConfirmPassword { get; set; }

    }

    public class AccountViewModel
    {
        public AccountRegisterViewModel Register { get; set; } = new AccountRegisterViewModel();
        public AccountLoginViewModel Login { get; set; } = new AccountLoginViewModel();

    }

    public class AccountLoginViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "נא להזין כתובת חוקית")]
        [EmailAddress(ErrorMessage = "נא להזין כתובת חוקית")]
        public string Email { get; set; }
       
        [Required(AllowEmptyStrings = false, ErrorMessage = "יש להקליד סיסמה")]
        public string Password { get; set; }

    }

    public class AccountRegisterViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "יש להזין שם מלא")]
        public string FullName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "יש להקליד סיסמה")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "הסיסמאות לא תואמות")]
        public string ConfirmPassword { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "נא להזין כתובת חוקית")]
        [EmailAddress(ErrorMessage = "נא להזין כתובת חוקית")]
        public string Email { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "נא להזין מספר חוקי")]
        [Phone(ErrorMessage = "נא להזין מספר חוקי")]
        public string PhoneNumber { get; set; }
      
        [Range(1900, int.MaxValue, ErrorMessage = "ניתן להזין מספרים חיוביים מעל 1900")]
        public int YearOfBirth { get; set; }
        public bool IsAllowingPromotions { get; set; }
    }
}

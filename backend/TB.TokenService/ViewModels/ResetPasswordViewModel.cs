using System.ComponentModel.DataAnnotations;

namespace TB.TokenService.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "PasswordRequired")]
        [StringLength(100, ErrorMessage = "PasswordInvalidLength", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "PasswordsDoNotMatch")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
        public int UserId { get; set; }
    }
}

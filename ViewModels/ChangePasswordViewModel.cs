using System.ComponentModel.DataAnnotations;

namespace MentalTrack.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]

        public string Password { get; set; }
        [StringLength(40, MinimumLength = 8, ErrorMessage = "the {0} must be at {2} and at mas {1} character")]
        [DataType(DataType.Password)]
        [Display(Name ="New Password")]
        [Compare("ConfirmNewPassword", ErrorMessage = "Password does not match")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]

        public string ConfirmPassword { get; set; }

    }
}

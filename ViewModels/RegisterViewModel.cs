using System.ComponentModel.DataAnnotations;

namespace MentalTrack.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage ="Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Email is required")]

        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]

        public string Password { get; set; }
        [StringLength(40,MinimumLength =8,ErrorMessage ="the {0} must be at {2} and at mas {1} character")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "PhoneNum is required")]

        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]

        public string ConfirmPassword { get; set; }
    }
}

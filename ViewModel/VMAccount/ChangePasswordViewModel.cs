
using System.ComponentModel.DataAnnotations;

namespace CemexExamApp.ViewModel.VMAccount
{
    
    public class ChangePasswordViewModel
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; } = null!;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("NewPassword",ErrorMessage ="The new password and confirm password do not match.")]
        public string ConfirmNewPassword { get; set; } = null!;
    }
}

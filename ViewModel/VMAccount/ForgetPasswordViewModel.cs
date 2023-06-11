using System.ComponentModel.DataAnnotations;

namespace CemexExamApp.ViewModel.VMAccount
{
    public class ForgetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}

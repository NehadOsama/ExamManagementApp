using CemexExamApp.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CemexExamApp.ViewModel
{
    public class UserViewModel
    {

        [Key]
        public int ID { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "English character only allowed.")]

        [DisplayName("First Name")]
        public string FirstName { get; set; } = null!;

        [Required]
        [RegularExpression(@"^[a-zA-Z'' ']+$", ErrorMessage = "English character only allowed.")]
        [DisplayName("Full Name")]
        public string FullName { get; set; } = null!;

        [Required]
        [RegularExpression(@"^[a-zA-Z\0-9\\_]+$", ErrorMessage = "English character , 0:9 , special characters -_.!%# only allowed.")]
        [DisplayName("Username")]
        public string Username { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [StringLength(11, MinimumLength = 11, ErrorMessage = "The Mobile Number should be 11 number.")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Please enter numbers only")]
        public string Mobile { get; set; } = null!;
        public int RoleID { get; set; }
        public bool Active { get; set; }
        public bool AD { get; set; }

        [NotMapped]
        [DisplayName("Profile Picture")]
        public IFormFile? ProfilePic { get; set; }
        public string? SavedPicPath { get; set; }
        public string? Password { get; set; }

        [DisplayName("Created By")]
        public string? CreatedBy { get; set; }

        [Column(TypeName = "datetime")]
        [DisplayName("Created Date")]
        public DateTime? CreatedDate { get; set; }

        [DisplayName("Last Update by")]
        public string? LastUpdatedBy { get; set; }

        [Column(TypeName = "datetime")]
        [DisplayName("Last Update Date")]
        public DateTime? LastUpdatedDate { get; set; }

    }
}

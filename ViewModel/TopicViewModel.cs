using System.ComponentModel.DataAnnotations;

namespace CemexExamApp.ViewModel
{
    public class TopicViewModel
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z\0-9\\_\\?]+$", ErrorMessage = "English character , 0:9 , special characters -_.!%#? only allowed.")]
        public string EnglishName { get; set; } = null!;

        [Required]
        [RegularExpression(@"^[\u0621-\u064A\u0660-\u0669\0-9\\_\\؟]+$", ErrorMessage = "Arabic character , 0:9 , special characters -_.؟ only allowed.")]
        public string ArabicName{ get; set; } = null!;
    }
}

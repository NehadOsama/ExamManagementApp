
//using ExpressiveAnnotations.Attributes;
using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;
using UoN.ExpressiveAnnotations.NetCore.Attributes;
//using System.ComponentModel.DataAnnotations;

namespace CemexExamApp.ViewModel
{
    public class QuestionAnswerViewModel
    {

        [Key]
        public long ID { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
       //[RegularExpression(@"^[a-zA-Z0-9_-.'' ']", ErrorMessage = "English character , 0:9 , special characters -_.? only allowed.")]
        [RegularExpression(@"^[a-zA-Z\0-9\\_\\?]+$", ErrorMessage = "English character , 0:9 , special characters -_.!%#? only allowed.")]
        public string EnglishName { get; set; } 
        [System.ComponentModel.DataAnnotations.Required]
        [RegularExpression(@"^[\u0621-\u064A\u0660-\u0669\0-9\\_\\؟]+$", ErrorMessage = "Arabic character , 0:9 , special characters -_.؟ only allowed.")]
        //  [RegularExpression(@"^[\u0600-\u06FF*$/'' ' _-.]", ErrorMessage = "Arabic character , 0:9 , special characters -_.؟ only allowed.")]
        public string ArabicName { get; set; }
        public int TopicID { get; set; }
        public int LevelID { get; set; }
        public int AnswerCountID { get; set; }
        public bool Active { get; set; }
        public int CorrectAnswerID { get; set; }


        [RegularExpression(@"^[a-zA-Z\0-9\\_\\?]+$", ErrorMessage = "English character , 0:9 , special characters -_.!%#? only allowed.")]
        public string EnAns1 { get; set; } = null!;


        [RegularExpression(@"^[a-zA-Z\0-9\\_\\?]+$", ErrorMessage = "English character , 0:9 , special characters -_.!%#? only allowed.")]
        public string EnAns2 { get; set; } = null!;


        [RegularExpression(@"^[a-zA-Z\0-9\\_\\?]+$", ErrorMessage = "English character , 0:9 , special characters -_.!%#? only allowed.")]
        public string EnAns3 { get; set; } = null!;


        [RequiredIf("(AnswerCountID == 4 || AnswerCountID == 5)", ErrorMessage = "The EnAns4 is required.")]
        [RegularExpression(@"^[a-zA-Z\0-9\\_\\?]+$", ErrorMessage = "English character , 0:9 , special characters -_.!%#? only allowed.")]
        public string? EnAns4 { get; set; }


        [RequiredIf("AnswerCountID == 5", ErrorMessage = "The EnAns5 is required.")]
        [RegularExpression(@"^[a-zA-Z\0-9\\_\\?]+$", ErrorMessage = "English character , 0:9 , special characters -_.!%#? only allowed.")]
        public string? EnAns5 { get; set; }


        [RegularExpression(@"^[\u0621-\u064A\u0660-\u0669\0-9\\_\\؟]+$", ErrorMessage = "Arabic character , 0:9 , special characters -_.؟ only allowed.")]
        public string ArAns1 { get; set; } = null!;


        [RegularExpression(@"^[\u0621-\u064A\u0660-\u0669\0-9\\_\\؟]+$", ErrorMessage = "Arabic character , 0:9 , special characters -_.؟ only allowed.")]
        public string ArAns2 { get; set; } = null!;


        [RegularExpression(@"^[\u0621-\u064A\u0660-\u0669\0-9\\_\\؟]+$", ErrorMessage = "Arabic character , 0:9 , special characters -_.؟ only allowed.")]
        public string ArAns3 { get; set; } = null!;

        
        [RequiredIf("(AnswerCountID == 4 || AnswerCountID == 5)", ErrorMessage = "The ArAns4 field is required.")]
        [RegularExpression(@"^[\u0621-\u064A\u0660-\u0669\0-9\\_\\؟]+$", ErrorMessage = "Arabic character , 0:9 , special characters -_.؟ only allowed.")]
        public string? ArAns4 { get; set; }


        [RequiredIf("AnswerCountID == 5", ErrorMessage = "The ArAns5 is required.")]
        [RegularExpression(@"^[\u0621-\u064A\u0660-\u0669\0-9\\_\\؟]+$", ErrorMessage = "Arabic character , 0:9 , special characters -_.؟ only allowed.")]
        public string? ArAns5 { get; set; }

    }
}

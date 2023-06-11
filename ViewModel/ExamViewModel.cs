using CemexExamApp.Models;
using ExpressiveAnnotations.Attributes;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace CemexExamApp.ViewModel
{
    public class ExamViewModel
    {

        [Key]
        public long ExamID { get; set; }

        [Required]
        [DisplayName("Training Name")]
        public string TrainingName  { get; set; } = null!;

        [Column(TypeName = "date")]
        [DisplayName("Training From date")]
        public DateTime TrainingFromDate { get; set; }

        [Column(TypeName = "date")]
        [DisplayName("Training To date")]
        public DateTime TrainingToDate { get; set; }

        [DisplayName("Exam Language")]
        public int LanguageID { get; set; }

        [DisplayName("Difficulty level")]
        public int LevelID { get; set; }

        public Level? level { get; set; }

        [DisplayName("Benchmark")]
        public int BenchmarkID { get; set; }

        [DisplayName("Exam Duration")]
        public int DurationID { get; set; }

        [Column(TypeName = "date")]
        [DisplayName("Exam Validity From Date")]
        public DateTime ValidityDateFrom { get; set; }

        [Column(TypeName = "date")]
        [DisplayName("Exam Validity To Date")]
        public DateTime ValidityDateTo { get; set; }

        [NotMapped]
        public IFormFile TakersSheet { get; set; } = null!;

        [DisplayName("Topics")]
        public List<Int32> TopicsList { get; set; } = null!;

        [DisplayName("Number of Questions ")]
        public int NumberOfQuestions { get; set; }
        [DisplayName("Number of Topics ")]
        public int NumberOfTopics { get; set; }
        [DisplayName("Number of Questions per each Topic  ")]
        public int NumberOfQuestionsPerTopic { get; set; }

        public bool NeedUpdateSheet { get; set; }
    }
}

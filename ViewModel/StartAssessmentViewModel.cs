using System.ComponentModel.DataAnnotations;

namespace CemexExamApp.ViewModel
{
    public class StartAssessmentViewModel
    {
        [Key]
        public long ExamId { get; set; }

        public long ExamTakerId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        //public bool IsValid { get; set; }

        public string? TakerName { get; set; }
        public string? QuestionsCount { get; set; }
        public string? Duration { get; set; }
        public DateTime? ValidityDateTo { get; set; }

    }
}

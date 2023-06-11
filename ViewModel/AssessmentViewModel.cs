using CemexExamApp.Models;
using MessagePack;
using System.Diagnostics.CodeAnalysis;

namespace CemexExamApp.ViewModel
{
    public class AssessmentViewModel
    {
        public long ExamTakerId { get; set; }
        public Exam exam { get; set; } = null!;
        public List<QuestionwithAns> QuestionwithAns { get; set; } = null!;
        [AllowNull]
        public List<TakerAnswer> takerAnswer { get; set; }
    }

    public class QuestionwithAns
    {
        public Question question { get; set; } = null!;
        public QuestionAnswer QquAnswers { get; set; } = null!;
         
    }

    public class TakerAnswer
    {
        public long questionId { get; set; } 
        public int AnswerNo { get; set; } 
        public bool IsCorrect { get; set; }

        public int CorrectAnswerNo { get; set; }
    }
}

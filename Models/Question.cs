using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CemexExamApp.Models
{
    [Table("Question")]
    public partial class Question
    {
        public Question()
        {
            ExamQuestions = new HashSet<ExamQuestion>();
            ExamTakerAnswers = new HashSet<ExamTakerAnswer>();
            QuestionAnswers = new HashSet<QuestionAnswer>();
        }

        [Key]
        public long ID { get; set; }
        public string EnglishName { get; set; } = null!;
        public string ArabicName { get; set; } = null!;
        public int TopicID { get; set; }
        public int LevelID { get; set; }
        public int AnswerCountID { get; set; }
        public bool Active { get; set; }
        public int CorrectAnswerID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastUpdatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }

        [ForeignKey("AnswerCountID")]
        [InverseProperty("QuestionAnswerCounts")]
        public virtual AnswerCount AnswerCount { get; set; } = null!;
        [ForeignKey("CorrectAnswerID")]
        [InverseProperty("QuestionCorrectAnswers")]
        public virtual AnswerCount CorrectAnswer { get; set; } = null!;
        [ForeignKey("LevelID")]
        [InverseProperty("Questions")]
        public virtual Level Level { get; set; } = null!;
        [ForeignKey("TopicID")]
        [InverseProperty("Questions")]
        public virtual Topic Topic { get; set; } = null!;
        [InverseProperty("Question")]
        public virtual ICollection<ExamQuestion> ExamQuestions { get; set; }
        [InverseProperty("Question")]
        public virtual ICollection<ExamTakerAnswer> ExamTakerAnswers { get; set; }
        [InverseProperty("Question")]
        public virtual ICollection<QuestionAnswer> QuestionAnswers { get; set; }
    }
}

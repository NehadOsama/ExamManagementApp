using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CemexExamApp.Models
{
    [Table("Exam")]
    public partial class Exam
    {
        public Exam()
        {
            ExamQuestions = new HashSet<ExamQuestion>();
            ExamTakers = new HashSet<ExamTaker>();
            ExamTopics = new HashSet<ExamTopic>();
            training = new HashSet<training>();
        }

        [Key]
        public long ID { get; set; }
        public int LanguageID { get; set; }
        public int LevelID { get; set; }
        public int BenchmarkID { get; set; }
        public int QuestionsCount { get; set; }
        public int TopicsCount { get; set; }
        public int QuestionPerTopicCount { get; set; }
        public int DurationID { get; set; }
        [Column(TypeName = "date")]
        public DateTime ValidityDateFrom { get; set; }
        [Column(TypeName = "date")]
        public DateTime ValidityDateTo { get; set; }
        public string? TakersSheetPath { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastUpdatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }

        [ForeignKey("BenchmarkID")]
        [InverseProperty("Exams")]
        public virtual Benchmark Benchmark { get; set; } = null!;
        [ForeignKey("DurationID")]
        [InverseProperty("Exams")]
        public virtual Duration Duration { get; set; } = null!;
        [ForeignKey("LanguageID")]
        [InverseProperty("Exams")]
        public virtual Language Language { get; set; } = null!;
        [ForeignKey("LevelID")]
        [InverseProperty("Exams")]
        public virtual Level Level { get; set; } = null!;
        [InverseProperty("Exam")]
        public virtual ICollection<ExamQuestion> ExamQuestions { get; set; }
        [InverseProperty("Exam")]
        public virtual ICollection<ExamTaker> ExamTakers { get; set; }
        [InverseProperty("Exam")]
        public virtual ICollection<ExamTopic> ExamTopics { get; set; }
        [InverseProperty("Exam")]
        public virtual ICollection<training> training { get; set; }
    }
}

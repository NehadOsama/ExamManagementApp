using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CemexExamApp.Models
{
    [Table("ExamQuestion")]
    public partial class ExamQuestion
    {
        [Key]
        public long ID { get; set; }
        public long ExamID { get; set; }
        public long QuestionID { get; set; }
        [StringLength(50)]
        public string? mark { get; set; }

        [ForeignKey("ExamID")]
        [InverseProperty("ExamQuestions")]
        public virtual Exam Exam { get; set; } = null!;
        [ForeignKey("QuestionID")]
        [InverseProperty("ExamQuestions")]
        public virtual Question Question { get; set; } = null!;
    }
}

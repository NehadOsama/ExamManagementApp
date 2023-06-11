using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CemexExamApp.Models
{
    [Table("ExamTakerQuestion")]
    public partial class ExamTakerQuestion
    {
        [Key]
        public long ID { get; set; }
        public long QuestionID { get; set; }
        public long ExamTakerID { get; set; }
        public int? AnsNo { get; set; }
        public bool? IsCorrect { get; set; }

        [ForeignKey("ExamTakerID")]
        [InverseProperty("ExamTakerQuestions")]
        public virtual ExamTaker ExamTaker { get; set; } = null!;
        [ForeignKey("QuestionID")]
        [InverseProperty("ExamTakerQuestions")]
        public virtual Question Question { get; set; } = null!;
    }
}

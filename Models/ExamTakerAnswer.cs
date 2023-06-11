using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CemexExamApp.Models
{
    [Table("ExamTakerAnswer")]
    public partial class ExamTakerAnswer
    {
        [Key]
        public long ID { get; set; }
        public long ExamTakerID { get; set; }
        public long QuestionID { get; set; }
        public int AnsNo { get; set; }
        public bool IsCorrect { get; set; }

        [ForeignKey("ExamTakerID")]
        [InverseProperty("ExamTakerAnswers")]
        public virtual ExamTaker ExamTaker { get; set; } = null!;
        [ForeignKey("QuestionID")]
        [InverseProperty("ExamTakerAnswers")]
        public virtual Question Question { get; set; } = null!;
    }
}

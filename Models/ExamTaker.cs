using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CemexExamApp.Models
{
    [Table("ExamTaker")]
    public partial class ExamTaker
    {
        public ExamTaker()
        {
            ExamTakerAnswers = new HashSet<ExamTakerAnswer>();
        }

        [Key]
        public long ID { get; set; }
        public long ExamID { get; set; }
        public string Username { get; set; } = null!;
        public string EMail { get; set; } = null!;
        public bool? FinalResult { get; set; }
        public bool OpenedBefore { get; set; }

        [ForeignKey("ExamID")]
        [InverseProperty("ExamTakers")]
        public virtual Exam Exam { get; set; } = null!;
        [InverseProperty("ExamTaker")]
        public virtual ICollection<ExamTakerAnswer> ExamTakerAnswers { get; set; }
    }
}

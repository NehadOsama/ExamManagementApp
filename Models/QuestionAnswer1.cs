using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CemexExamApp.Models
{
    [Table("QuestionAnswer1")]
    public partial class QuestionAnswer1
    {
        [Key]
        public long ID { get; set; }
        public long QuestionID { get; set; }
        public int AnswerNo { get; set; }
        public string EnAns { get; set; } = null!;
        public string ArAns { get; set; } = null!;

        [ForeignKey("QuestionID")]
        [InverseProperty("QuestionAnswer1s")]
        public virtual Question Question { get; set; } = null!;
    }
}

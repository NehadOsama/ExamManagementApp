using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CemexExamApp.Models
{
    [Table("QuestionAnswer")]
    public partial class QuestionAnswer
    {
        [Key]
        public long ID { get; set; }
        public long QuestionID { get; set; }
        public string EnAns1 { get; set; } = null!;
        public string EnAns2 { get; set; } = null!;
        public string EnAns3 { get; set; } = null!;
        public string? EnAns4 { get; set; }
        public string? EnAns5 { get; set; }
        public string ArAns1 { get; set; } = null!;
        public string ArAns2 { get; set; } = null!;
        public string ArAns3 { get; set; } = null!;
        public string? ArAns4 { get; set; }
        public string? ArAns5 { get; set; }

        [ForeignKey("QuestionID")]
        [InverseProperty("QuestionAnswers")]
        public virtual Question Question { get; set; } = null!;
    }
}

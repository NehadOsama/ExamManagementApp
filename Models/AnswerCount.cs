using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CemexExamApp.Models
{
    [Table("AnswerCount")]
    public partial class AnswerCount
    {
        public AnswerCount()
        {
            QuestionAnswerCounts = new HashSet<Question>();
            QuestionCorrectAnswers = new HashSet<Question>();
        }

        [Key]
        public int ID { get; set; }
        public int Count { get; set; }

        [InverseProperty("AnswerCount")]
        public virtual ICollection<Question> QuestionAnswerCounts { get; set; }
        [InverseProperty("CorrectAnswer")]
        public virtual ICollection<Question> QuestionCorrectAnswers { get; set; }
    }
}

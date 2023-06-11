using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CemexExamApp.Models
{
    [Table("QuestionCount")]
    public partial class QuestionCount
    {
        public QuestionCount()
        {
            Exams = new HashSet<Exam>();
        }

        [Key]
        public int ID { get; set; }
        public int Count { get; set; }

        [InverseProperty("QuestionCount")]
        public virtual ICollection<Exam> Exams { get; set; }
    }
}

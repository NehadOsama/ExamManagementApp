using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CemexExamApp.Models
{
    [Table("QuestionPerTopicCount")]
    public partial class QuestionPerTopicCount
    {
        public QuestionPerTopicCount()
        {
            Exams = new HashSet<Exam>();
        }

        [Key]
        public int ID { get; set; }
        public int Count { get; set; }

        [InverseProperty("QuestionPerTopicCount")]
        public virtual ICollection<Exam> Exams { get; set; }
    }
}

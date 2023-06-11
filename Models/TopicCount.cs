using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CemexExamApp.Models
{
    [Table("TopicCount")]
    public partial class TopicCount
    {
        public TopicCount()
        {
            Exams = new HashSet<Exam>();
        }

        [Key]
        public int ID { get; set; }
        public int Count { get; set; }

        [InverseProperty("TopicCount")]
        public virtual ICollection<Exam> Exams { get; set; }
    }
}

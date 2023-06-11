using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CemexExamApp.Models
{
    [Table("Level")]
    public partial class Level
    {
        public Level()
        {
            Exams = new HashSet<Exam>();
            Questions = new HashSet<Question>();
        }

        [Key]
        public int ID { get; set; }
        public string Name { get; set; } = null!;

        [InverseProperty("Level")]
        public virtual ICollection<Exam> Exams { get; set; }
        [InverseProperty("Level")]
        public virtual ICollection<Question> Questions { get; set; }
    }
}

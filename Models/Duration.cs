using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CemexExamApp.Models
{
    [Table("Duration")]
    public partial class Duration
    {
        public Duration()
        {
            Exams = new HashSet<Exam>();
        }

        [Key]
        public int ID { get; set; }
        [Column("Duration")]
        public int Duration1 { get; set; }

        [InverseProperty("Duration")]
        public virtual ICollection<Exam> Exams { get; set; }
    }
}

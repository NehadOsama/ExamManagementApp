using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CemexExamApp.Models
{
    [Table("Benchmark")]
    public partial class Benchmark
    {
        public Benchmark()
        {
            Exams = new HashSet<Exam>();
        }

        [Key]
        public int ID { get; set; }
        [StringLength(10)]
        public string Name { get; set; } = null!;

        [InverseProperty("Benchmark")]
        public virtual ICollection<Exam> Exams { get; set; }
    }
}

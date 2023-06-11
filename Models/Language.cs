using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CemexExamApp.Models
{
    [Table("Language")]
    public partial class Language
    {
        public Language()
        {
            Exams = new HashSet<Exam>();
        }

        [Key]
        public int ID { get; set; }
        [StringLength(50)]
        public string Name { get; set; } = null!;

        [InverseProperty("Language")]
        public virtual ICollection<Exam> Exams { get; set; }
    }
}

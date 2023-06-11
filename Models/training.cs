using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CemexExamApp.Models
{
    [Table("Training")]
    public partial class training
    {
        [Key]
        public long ID { get; set; }
        public string Name { get; set; } = null!;
        [Column(TypeName = "date")]
        public DateTime FromDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime ToDate { get; set; }
        public long ExamID { get; set; }

        [ForeignKey("ExamID")]
        [InverseProperty("training")]
        public virtual Exam Exam { get; set; } = null!;
    }
}

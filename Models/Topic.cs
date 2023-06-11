using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CemexExamApp.Models
{
    [Table("Topic")]
    public partial class Topic
    {
        public Topic()
        {
            ExamTopics = new HashSet<ExamTopic>();
            Questions = new HashSet<Question>();
        }

        [Key]
        public int ID { get; set; }
        public string EnglishName { get; set; } = null!;
        public string ArabicName { get; set; } = null!;
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastUpdatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }

        [InverseProperty("Topic")]
        public virtual ICollection<ExamTopic> ExamTopics { get; set; }
        [InverseProperty("Topic")]
        public virtual ICollection<Question> Questions { get; set; }
    }
}

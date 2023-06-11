using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CemexExamApp.Models
{
    [Table("ExamTopic")]
    public partial class ExamTopic
    {
        [Key]
        public long ID { get; set; }
        public long ExamID { get; set; }
        public int TopicID { get; set; }

        [ForeignKey("ExamID")]
        [InverseProperty("ExamTopics")]
        public virtual Exam Exam { get; set; } = null!;
        [ForeignKey("TopicID")]
        [InverseProperty("ExamTopics")]
        public virtual Topic Topic { get; set; } = null!;
    }
}

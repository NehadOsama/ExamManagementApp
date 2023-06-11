using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CemexExamApp.Models
{
    [Table("ExceptionLog")]
    public partial class ExceptionLog
    {
        [Key]
        public long ID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LogDate { get; set; }
        [StringLength(100)]
        public string? LogUserName { get; set; }
        public int? LogUserId { get; set; }
        public string? ExceptionMessage { get; set; }
        public string? InnerException { get; set; }
        public string? StackTrace { get; set; }
        [StringLength(200)]
        public string? PageURL { get; set; }
        [StringLength(50)]
        public string? IPAddress { get; set; }
    }
}

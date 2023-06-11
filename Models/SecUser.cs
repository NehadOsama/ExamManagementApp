using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CemexExamApp.Models
{
    [Table("SecUser")]
    public partial class SecUser
    {
        [Key]
        public int ID { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        [StringLength(50)]
        public string Mobile { get; set; } = null!;
        public int RoleID { get; set; }
        public bool Active { get; set; }
        public bool AD { get; set; }
        public string? ProfilePic { get; set; }
        public string? Password { get; set; }
        public string? CreatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedDate { get; set; }
        public string? LastUpdatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastUpdatedDate { get; set; }

        [ForeignKey("RoleID")]
        [InverseProperty("SecUsers")]
        public virtual Role Role { get; set; } = null!;
    }
}

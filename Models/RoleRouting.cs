using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CemexExamApp.Models
{
    [Table("RoleRouting")]
    public partial class RoleRouting
    {
        [Key]
        public int RoleId { get; set; }
        [Key]
        public int ControllerActionId { get; set; }
        [StringLength(50)]
        public string? mark { get; set; }

        [ForeignKey("ControllerActionId")]
        [InverseProperty("RoleRoutings")]
        public virtual ControllerAction ControllerAction { get; set; } = null!;
        [ForeignKey("RoleId")]
        [InverseProperty("RoleRoutings")]
        public virtual Role Role { get; set; } = null!;
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CemexExamApp.Models
{
    [Table("Role")]
    public partial class Role
    {
        public Role()
        {
            RoleRoutings = new HashSet<RoleRouting>();
            SecUsers = new HashSet<SecUser>();
        }

        [Key]
        public int ID { get; set; }
        public string Name { get; set; } = null!;

        [InverseProperty("Role")]
        public virtual ICollection<RoleRouting> RoleRoutings { get; set; }
        [InverseProperty("Role")]
        public virtual ICollection<SecUser> SecUsers { get; set; }
    }
}

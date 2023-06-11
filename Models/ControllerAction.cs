using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CemexExamApp.Models
{
    public partial class ControllerAction
    {
        public ControllerAction()
        {
            RoleRoutings = new HashSet<RoleRouting>();
        }

        [Key]
        public int ID { get; set; }
        public string ControllerName { get; set; } = null!;
        public string ActionName { get; set; } = null!;
        public string layoutName { get; set; } = null!;

        [InverseProperty("ControllerAction")]
        public virtual ICollection<RoleRouting> RoleRoutings { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using TechnicalSupportSystem.Models;

namespace TechnicalSupportSystem.Models
{
    public class Supervisor:Person
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int SupervisorID { get; set; }
        [StringLength(50)]    
        public string FirstName { get; set; }
        [StringLength(50)]    
        public string MiddleName { get; set; }
        [StringLength(50)]    
        public string LastName { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
        public virtual UserProfile UserProfile { get; set; }
    }
}
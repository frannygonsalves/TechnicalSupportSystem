using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Security;
using TechnicalSupportSystemV2.Models;

namespace TechnicalSupportSystem.Models
{
    public class Student:Person
    {

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int StudentID { get; set; }
        [StringLength(50)]       
        public string FirstName { get; set; }
        [StringLength(50)]       
        public string MiddleName { get; set; }
        [StringLength(50)]       
        public string LastName { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual Project Project { get; set; }
        public virtual UserProfile UserProfile { get; set; }

    }
}
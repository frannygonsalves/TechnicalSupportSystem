using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TechnicalSupportSystem.Models
{
    public class Project
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ProjectID { get; set; }

        [Required]
        [Display(Name="Project No")]
        public int ProjectNo { get; set; }
        
        [Display(Name="Module No")]
        [Required]
        public string Module { get; set; }
        
        [Display(Name= "Project Name")]
        [Required]
        public string  ProjectName { get; set; }
        
        [Display(Name= "Project Description")]
        public string ProjectDescription { get; set; }
        
        [Display(Name="Group Project?")]
        [Required]
        public bool IsGroupProject { get; set; }
        
        [Required]
        public decimal Budget { get; set; }
        public virtual ICollection<Student> Student { get; set; }
        public virtual Supervisor Supervisor { get; set; }
    }
}
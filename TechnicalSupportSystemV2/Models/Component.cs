using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TechnicalSupportSystem.Models
{
    public class Component
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ComponentID { get; set; }
        [Required]
        [StringLength(100,MinimumLength=2,ErrorMessage="Enter text in between 2-100 characters")]
        public string Name { get; set; }
        [Required]
        public string StockCode { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Enter text in between 3-100 characters")]
        public string Description { get; set; }
        [Required]
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public virtual Order Order { get; set; }
    }
}
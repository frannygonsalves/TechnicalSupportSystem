using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TechnicalSupportSystem.Models
{
    public class Order
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int OrderID { get; set; }
        public DateTime RequestDate { get; set; }
        public bool IsCollected { get; set; }
        public DateTime? CollectionDate { get; set; }
        public bool IsApproved { get; set; }
        public bool IsOverBudget {get;set;}
        public bool IsChecked { get; set; }
        public bool IsOrdered { get; set; }
        public decimal DeliveryCost { get; set; }
        public bool StudentIsNotified { get; set; }
        public int? VendorOrderNumber { get; set; }
        public string VendorName { get; set; }
        public DateTime? OrderDate { get; set; }
        public decimal ComponentTotal { get; set; }
        public decimal OrderTotal { get; set; }
        public string OrderedBy { get; set; }
        public virtual Student Student { get; set; }
        public virtual ICollection<Component> Components { get; set; }
    }
}
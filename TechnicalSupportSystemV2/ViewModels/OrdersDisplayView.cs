using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TechnicalSupportSystem.Models;

namespace TechnicalSupportSystem.ViewModels
{
    public class OrdersDisplayView
    {
        public IEnumerable<Order> Orders { get; set; }
        public decimal GrandTotal { get; set; }
    }
}
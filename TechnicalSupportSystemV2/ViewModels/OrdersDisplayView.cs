using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TechnicalSupportSystem.Models;

namespace TechnicalSupportSystemV2.ViewModels
{
    public class OrdersDisplayView
    {
        public IEnumerable<Order> Orders { get; set; }
        public decimal GrandTotal { get; set; }
    }
}
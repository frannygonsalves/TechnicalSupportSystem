using DotNet.Highcharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TechnicalSupportSystem.Models;


namespace TechnicalSupportSystem.ViewModels
{
    public class ProjectSpendView
    {
        public Highcharts BarChart { get; set; }
        public Highcharts PieChart { get; set; }

        public Project Project { get; set; }
        public IEnumerable<Order> Orders {get;set;}
        public decimal Budget { get; set; }
        public decimal TotalOrdersCost {get; set; }
        public decimal SpendLeft { get; set; }

    }
}
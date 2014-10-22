using DotNet.Highcharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TechnicalSupportSystemV2.Models;

namespace TechnicalSupportSystemV2.ViewModels
{
    public class TechnicianOrderOverview
    {
        public int OrdersWaitingToBeOrdered { get; set; }
        public int StudentsWhoNeedToBeNotified { get; set; }
        public int OrdersNotCollected { get; set; }
        public Technician Technician { get; set; }
        public Highcharts PieChart { get; set; }
    }
}
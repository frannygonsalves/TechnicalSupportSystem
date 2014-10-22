using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TechnicalSupportSystem.Models;

namespace TechnicalSupportSystemV2.Services
{
    public class TotalOrdersCostCalculator:ITotalOrdersCostCalculator
    {
        public decimal TotalCost (Student student)
        {
            var query = from o in student.Orders
                        where o.IsApproved == false
                        select o;

            decimal total = query.Sum(i => i.OrderTotal);

            return total;

        }
    }
}
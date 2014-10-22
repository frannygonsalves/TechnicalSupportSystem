using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TechnicalSupportSystem.Models;

namespace TechnicalSupportSystemV2.Services
{
    public class OrderInformation : IOrderInformation
    {

        public IEnumerable<Order> GetApprovedOrders(Student student)
        {
           var orders =  from o in student.Orders  // a query to retrieve aorders that are approved from student. orders object 
                         where o.IsApproved== true
                         select o;

           return orders;
        }


        public decimal GetTotalOrdersCost(IEnumerable<Order> orders)
        {
            return orders.Sum(i => i.OrderTotal);
        }
    }
}
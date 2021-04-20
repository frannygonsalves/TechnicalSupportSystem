using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechnicalSupportSystem.Models;

namespace TechnicalSupportSystem.Services
{
    public interface IOrderInformation
    {
       IEnumerable<Order> GetApprovedOrders(Student student);
       decimal GetTotalOrdersCost(IEnumerable<Order> order);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TechnicalSupportSystem.Models;

namespace TechnicalSupportSystem.Services
{
    public interface ITotalOrdersCostCalculator
    {
          decimal TotalCost(Student student);
    }
}
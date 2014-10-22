using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnicalSupportSystemV2.Models
{
     interface Person
    {
        string FirstName { get; set; }
        string MiddleName { get; set; }
        string LastName { get; set; }
    }
}

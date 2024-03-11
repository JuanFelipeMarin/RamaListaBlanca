using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Service.Entities.Classes
{
    public class eActionParameters
    {
        public int id { get; set; }
        public int actionId { get; set; }
        public string parameterName { get; set; }
        public string parameterValue { get; set; }
        public string dateParameter { get; set; }
        public int gymId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Classes
{
    public class eVisit
    {
        public string visitedPerson { get; set; }
        public string whoAuthorized { get; set; }
        public string reason { get; set; }
        public string elements { get; set; }
        public int time { get; set; }
    }
}

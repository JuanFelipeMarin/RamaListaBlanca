using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Service.Entities.Classes
{
    public class eAdditionalEntry
    {
        public int id { get; set; }
        public string reason { get; set; }
        public string entryDate { get; set; }
        public bool updated { get; set; }
        public string entryType { get; set; }
    }
}

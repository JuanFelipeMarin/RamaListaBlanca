using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Service.Entities.Classes
{
    public class eTerminalEvents
    {
        public string ipAddres { get; set; }
        public DateTime date { get; set; }
        public string userId { get; set; }
        public bool isExit { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Classes
{
    public class eMailServerData
    {
        public string SMTPServer { get; set; }
        public string user { get; set; }
        public string password { get; set; }
        public string port { get; set; }
    }
}

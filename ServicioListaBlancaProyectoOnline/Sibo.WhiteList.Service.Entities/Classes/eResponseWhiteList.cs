using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Service.Entities.Classes
{
    public class eResponseWhiteList
    {
        public eResponse response { get; set; }
        public eWhiteList whiteList { get; set; }
    }
}

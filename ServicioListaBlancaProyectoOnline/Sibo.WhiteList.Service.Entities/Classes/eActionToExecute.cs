using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Service.Entities.Classes
{
    public class eActionToExecute
    {
        public string ipAddress { get; set; }
        public List<eAction> actionLists { get; set; }
    }
}

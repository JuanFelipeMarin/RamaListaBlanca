using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Entities.Classes
{
   public class eAction
    {
        public int id { get; set; }
        public string ipAddress { get; set; }
        public string strAction { get; set; }
        public string dateAction { get; set; }
        public bool used { get; set; }
        public bool stateAction { get; set; }
        public string modifiedDate { get; set; }
        public int cdgimnasio { get; set; }
        public int intIdSucursales { get; set; }
    }

    public class eActionParameters
    {
        public int id { get; set; }
        public int actionId { get; set; }
        public string parameterName { get; set; }
        public string parameterValue { get; set; }
        public DateTime dateParameter { get; set; }
        public int gymId { get; set; }
    }
}

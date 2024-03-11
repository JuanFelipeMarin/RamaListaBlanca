using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Service.Entities.Classes
{
    public class ePlan
    {
        public int pla_codigo { get; set; }
        public string pla_descripc { get; set; }
        public int cdplusu { get; set; }
        public int plusu_numero_fact { get; set; }
        public string plusu_fecha_inicio { get; set; }
        public string plusu_fecha_vcto { get; set; }
    }
}

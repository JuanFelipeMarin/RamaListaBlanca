using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Classes
{
    public class eContratos
    {
        public int id { get; set; }
        public byte[] fingerPrint { get; set; }
        public int gymId { get; set; }
        public int finger { get; set; }
        public int quality { get; set; }
        public int branchId { get; set; }
        public string personId { get; set; }
    }

    public class eContratoConDetalle
    {
        public int? tipoContrato { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Classes
{
    public class eFingerprint
    {
        public int id { get; set; }
        public byte[] fingerPrint { get; set; }
        public int gymId { get; set; }
        public int finger { get; set; }
        public int quality { get; set; }
        public string branchId { get; set; }
        public string personId { get; set; }
        public string modoGrabacion { get; set; }
    }

    public class eFingerprintWL
    {
        public int recordId { get; set; }
        public int id { get; set; }
        public byte[] fingerPrint { get; set; }
        public int gymId { get; set; }
        public int finger { get; set; }
        public int quality { get; set; }
        public int branchId { get; set; }
        public string personId { get; set; }
        public string typePerson { get; set; }
        public string planId { get; set; }
        public string restrictions { get; set; }
        public string reserveId { get; set; }
        public string cardId { get; set; }
        public bool withoutFingerprint { get; set; }

        public string intIndiceHuellaActual { get; set; }
    }
}

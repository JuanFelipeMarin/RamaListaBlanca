using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Service.Entities.Classes
{
    public class eReplicatedFingerprint
    {
        public int id { get; set; }
        public int fingerprintId { get; set; }
        public string userId { get; set; }
        public string ipAddress { get; set; }
        public DateTime replicationDate { get; set; }
        public bool bitDelete { get; set; }
        public bool bitHuella { get; set; }
        public bool bitTarje_Pin { get; set; }
        public int cdgimnacio { get; set;  }
    }
}

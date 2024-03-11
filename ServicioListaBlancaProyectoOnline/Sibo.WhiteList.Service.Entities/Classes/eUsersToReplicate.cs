using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Service.Entities.Classes
{
    public class eUsersToReplicate
    {
        public string userId { get; set; }
        public string userName { get; set; }
        public int fingerprintId { get; set; }
        public bool withoutFingerprint { get; set; }
        public bool delete { get; set; }
        public bool insert { get; set; }
        public int recordId { get; set; }
        public string ipAddress { get; set; }
    }
}

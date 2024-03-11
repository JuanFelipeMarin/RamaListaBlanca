using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Classes
{
    public class eSaveSignedContract
    {
        public int gymId { get; set; }
        public int branchId { get; set; }
        public string userId { get; set; }
        public byte[] fingerprintImage { get; set; }
    }
}

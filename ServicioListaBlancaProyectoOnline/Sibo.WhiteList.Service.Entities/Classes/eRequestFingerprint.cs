using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Service.Entities.Classes
{
    public class eRequestFingerprint
    {
        public int gymId { get; set; }
        public string strData { get; set; }
        public byte[] imageBytes { get; set; }
    }
}

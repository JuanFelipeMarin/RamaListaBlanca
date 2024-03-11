using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Classes
{
    public class eSendClientContract
    {
        public string userId { get; set; }
        public string userEmail { get; set; }
        public string userName { get; set; }
        public int contractId { get; set; }
        public DateTime SignDate { get; set; }
        public int invoiceId { get; set; }
        public string documentType { get; set; }
        public string contractText { get; set; }
        public byte[] responsibleSignature { get; set; }
        public string contractType { get; set; }
        public string gymNit { get; set; }
        public string gymName { get; set; }
        public string gymAddress { get; set; }
        public string gymPhone { get; set; }
        public byte[] gymLogo { get; set; }
        public byte[] userFingerprint { get; set; }
        public string branchName { get; set; }
    }
}

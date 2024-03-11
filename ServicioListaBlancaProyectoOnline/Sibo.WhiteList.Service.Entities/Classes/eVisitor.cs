using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Service.Entities.Classes
{
    public class eVisitor
    {
        public int idPK { get; set; }
        public string visitorId { get; set; }
        public int idType { get; set; }
        public string name { get; set; }
        public string firstLastName { get; set; }
        public string secondLastName { get; set; }
        public DateTime bornDate { get; set; }
        public int genre { get; set; }
        public string phone { get; set; }
        public int eps { get; set; }
        public string address { get; set; }
        public string email { get; set; }
        public bool entryWithFingerprint { get; set; }
        public int gymId { get; set; }
        public int branchId { get; set; }
        public string userId { get; set; }
        public eVisit visit { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Entities.Classes
{
    public class eEntries
    {
        public int intPkId { get; set; }
        public string clientId { get; set; }
        public int planId { get; set; }
        public DateTime entryDate { get; set; }
        public DateTime entryHour { get; set; }
        public DateTime outDate { get; set; }
        public DateTime outHour { get; set; }
        public int branchId { get; set; }
        public int invoiceId { get; set; }
        public string documentType { get; set; }
        public bool discountTicket { get; set; }
        public int gymId { get; set; }
        public int visitId { get; set; }
    }
}

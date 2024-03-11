using System;

namespace Sibo.WhiteList.Service.Entities.Classes
{
    public class eEvent
    {
        public int intPkId { get; set; }
        public string clientId { get; set; }
        public string clientName { get; set; }
        public int planId { get; set; }
        public string planName { get; set; }
        public DateTime entryDate { get; set; }
        public DateTime entryHour { get; set; }
        public DateTime outDate { get; set; }
        public DateTime outHour { get; set; }
        public DateTime? modifiedDate { get; set; }
        public int branchId { get; set; }
        public int invoiceId { get; set; }
        public string documentType { get; set; }
        public bool discountTicket { get; set; }
        public int gymId { get; set; }
        public int visitId { get; set; }
        public string firstMessage { get; set; }
        public string secondMessage { get; set; }
        public string thirdMessage { get; set; }
        public string expirationDate { get; set; }
        public string dateLastEntry { get; set; }
        public bool successEntry { get; set; }
        public string ipAddress { get; set; }
        public string eventType { get; set; }
        public string qrCode { get; set; }
        public string temperature { get; set; }
        public string usuario { get; set; }
        public string strEmpresaIngresoAdicional { get; set; }
    }
}

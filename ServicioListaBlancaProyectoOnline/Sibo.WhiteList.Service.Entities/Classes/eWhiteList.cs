using System;
using System.Collections.Generic;

namespace Sibo.WhiteList.Service.Entities.Classes
{
    public class eWhiteList
    {
        public int intPkId { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public int planId { get; set; }
        public string planName { get; set; }
        public Nullable<System.DateTime> expirationDate { get; set; }
        public Nullable<System.DateTime> lastEntry { get; set; }
        public string planType { get; set; }
        public string typePerson { get; set; }
        public int availableEntries { get; set; }
        public string restrictions { get; set; }
        public int branchId { get; set; }
        public string branchName { get; set; }
        public int gymId { get; set; }
        public string personState { get; set; }
        public bool withoutFingerprint { get; set; }
        public Nullable<int> fingerprintId { get; set; }
        public byte[] fingerprint { get; set; }
        public string strDatoFoto { get; set; }
        public bool updateFingerprint { get; set; }
        public bool know { get; set; }
        public bool courtesy { get; set; }
        public bool groupEntriesControl { get; set; }
        public int groupEntriesQuantity { get; set; }
        public int groupId { get; set; }
        public bool isRestrictionClass { get; set; }
        public string classSchedule { get; set; }
        public Nullable<System.DateTime> dateClass { get; set; }
        public int reserveId { get; set; }
        public string className { get; set; }
        public int utilizedMegas { get; set; }
        public int utilizedTickets { get; set; }
        public string employeeName { get; set; }
        public string classIntensity { get; set; }
        public string classState { get; set; }
        public string photoPath { get; set; }
        public int invoiceId { get; set; }
        public int dianId { get; set; }
        public string documentType { get; set; }
        public int subgroupId { get; set; }
        public string cardId { get; set; }
        public string tipo { get; set; }
        public List<eClientCard> tblCardId { get; set; }
        public int cantidadhuellas { get; set; }
        public List<eClientesPalmas> tblPalmasId { get; set; }
    }
}

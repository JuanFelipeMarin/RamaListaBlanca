﻿using System;

namespace Sibo.WhiteList.Service.Entities.Classes
{
    public class eEntries
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
        public int branchId { get; set; }
        public int invoiceId { get; set; }
        public string documentType { get; set; }
        public bool discountTicket { get; set; }
        public int gymId { get; set; }
        public int visitId { get; set; }
        public string firstMessage { get; set; }
        public string secondMessage { get; set; }
        public string expirationDate { get; set; }
        public string dateLastEntry { get; set; }
        public bool successEntry { get; set; }
        public string ipAddress { get; set; }
        public int cantidadREgistrosEntrada { get; set; }
    }
}
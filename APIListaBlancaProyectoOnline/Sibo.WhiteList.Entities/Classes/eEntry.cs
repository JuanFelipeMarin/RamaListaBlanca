﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Classes
{
    public class eEntry
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
        public string qrCode { get; set; }
        public string temperature { get; set; }
        public string thirdMessage { get; set; }
        public string secondMessage { get; set; }
        public string usuario { get; set; }
        public string ipAddress { get; set; }
        public string clientName { get; set; }
        public string strEmpresaIngresoAdicional { get; set; }
        public int cantidadREgistrosEntrada { get; set; }

    }
}
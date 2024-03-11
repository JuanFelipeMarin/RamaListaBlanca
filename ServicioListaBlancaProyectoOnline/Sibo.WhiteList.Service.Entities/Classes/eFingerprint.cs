﻿using System;

namespace Sibo.WhiteList.Service.Entities.Classes
{
    public class eFingerprint
    {
        public int recordId { get; set; }
        public int id { get; set; }
        public byte[] fingerPrint { get; set; }
        public int gymId { get; set; }
        public int finger { get; set; }
        public int quality { get; set; }
        public int branchId { get; set; }
        public string personId { get; set; }
        public string typePerson { get; set; }
        public string planId { get; set; }
        public string restrictions { get; set;  }
        public string reserveId { get; set; }
        public string cardId { get; set; }
        public bool withoutFingerprint { get; set; }

        public string intIndiceHuellaActual { get; set; }
        public string modoGrabacion { get; set; }
    }

    public class eFingerprintTable
    {
        public string personId { get; set; }
        public int fingerprintId { get; set; }
        public byte[] fingerprint { get; set; }
        public string strDatoFoto { get; set; }
        public bool insert { get; set; }
        public bool delete { get; set; }
        public DateTime registerDate { get; set; }
        public string intIndiceHuellaActual { get; set; }
    }
}
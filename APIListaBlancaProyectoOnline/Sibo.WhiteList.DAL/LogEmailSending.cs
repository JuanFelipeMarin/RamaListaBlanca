//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sibo.WhiteList.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class LogEmailSending
    {
        public int intPkId { get; set; }
        public System.DateTime dateSending { get; set; }
        public string clientId { get; set; }
        public string clientEmail { get; set; }
        public string emailType { get; set; }
        public string template { get; set; }
        public string strContract { get; set; }
        public Nullable<int> invoiceId { get; set; }
        public Nullable<int> planId { get; set; }
        public Nullable<int> contractType { get; set; }
        public bool modified { get; set; }
        public string bits_replica { get; set; }
        public int cdgimnasio { get; set; }
    }
}

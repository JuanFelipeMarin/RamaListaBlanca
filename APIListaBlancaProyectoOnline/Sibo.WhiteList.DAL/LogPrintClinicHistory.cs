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
    
    public partial class LogPrintClinicHistory
    {
        public int LogPrintClinicHistoryID { get; set; }
        public string Reason { get; set; }
        public System.DateTime PrintDate { get; set; }
        public int MedicalConsultationID { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
    
        public virtual MedicalConsultation MedicalConsultation { get; set; }
    }
}
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
    
    public partial class ConsultationMedicine
    {
        public int ConsultationMedicineID { get; set; }
        public int MedicalConsultationID { get; set; }
        public int MedicineID { get; set; }
        public string ConsumptionTime { get; set; }
        public string Dose { get; set; }
        public string Purpose { get; set; }
    
        public virtual MedicalConsultation MedicalConsultation { get; set; }
        public virtual Medicine Medicine { get; set; }
    }
}

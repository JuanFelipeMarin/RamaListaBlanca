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
    
    public partial class ConsultationHipRiskHigherKinetics
    {
        public int ConsultationHipRiskHigherKineticsID { get; set; }
        public int MedicalConsultationID { get; set; }
        public int HipRiskHigherKineticsID { get; set; }
        public Nullable<int> Value { get; set; }
    
        public virtual SuperiorHipKineticRisk SuperiorHipKineticRisk { get; set; }
        public virtual MedicalConsultation MedicalConsultation { get; set; }
    }
}

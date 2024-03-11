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
    
    public partial class TypeConsultation
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TypeConsultation()
        {
            this.CUP = new HashSet<CUP>();
            this.MedicalConsultationDetailDoctor = new HashSet<MedicalConsultationDetailDoctor>();
            this.MedicalConsultationDetailEvaluator = new HashSet<MedicalConsultationDetailEvaluator>();
            this.MedicalConsultationDetailNutritionist = new HashSet<MedicalConsultationDetailNutritionist>();
            this.MedicalConsultationDetailPhysiotherapist = new HashSet<MedicalConsultationDetailPhysiotherapist>();
        }
    
        public int TypeConsultationID { get; set; }
        public string Description { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CUP> CUP { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MedicalConsultationDetailDoctor> MedicalConsultationDetailDoctor { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MedicalConsultationDetailEvaluator> MedicalConsultationDetailEvaluator { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MedicalConsultationDetailNutritionist> MedicalConsultationDetailNutritionist { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MedicalConsultationDetailPhysiotherapist> MedicalConsultationDetailPhysiotherapist { get; set; }
    }
}

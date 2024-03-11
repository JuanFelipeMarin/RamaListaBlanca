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
    
    public partial class AdminFeeConfiguration
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AdminFeeConfiguration()
        {
            this.PeriodDetailBuy = new HashSet<PeriodDetailBuy>();
            this.ConfigDetailAdditionalCashingToAdmin = new HashSet<ConfigDetailAdditionalCashingToAdmin>();
        }
    
        public int afc_intId { get; set; }
        public int per_intId { get; set; }
        public int afc_intPeriodsNumber { get; set; }
        public string afc_strDescription { get; set; }
        public bool afc_bitExpirationPeriod { get; set; }
        public bool afc_bitState { get; set; }
        public System.DateTime afc_dtmDateApply { get; set; }
        public string afc_strModifiedUser { get; set; }
        public System.DateTime afc_dtmModifiedDate { get; set; }
        public Nullable<bool> afc_bitModified { get; set; }
        public int cdgimnasio { get; set; }
        public string bits_replica { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PeriodDetailBuy> PeriodDetailBuy { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ConfigDetailAdditionalCashingToAdmin> ConfigDetailAdditionalCashingToAdmin { get; set; }
        public virtual Periodicity Periodicity { get; set; }
    }
}
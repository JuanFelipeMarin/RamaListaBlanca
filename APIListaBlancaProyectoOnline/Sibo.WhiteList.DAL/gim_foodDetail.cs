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
    
    public partial class gim_foodDetail
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public gim_foodDetail()
        {
            this.gim_foodDetailDiet = new HashSet<gim_foodDetailDiet>();
        }
    
        public int PK_foodDetailId { get; set; }
        public int FK_foodTypeDetailId { get; set; }
        public int FK_foodId { get; set; }
        public int FK_foodMeasurementId { get; set; }
        public int FK_portionTypeId { get; set; }
        public int cdgimnasio { get; set; }
        public int foodOrder { get; set; }
        public string unitNumber { get; set; }
        public string portionNumber { get; set; }
    
        public virtual gim_foodMeasurements gim_foodMeasurements { get; set; }
        public virtual gim_foods gim_foods { get; set; }
        public virtual gim_foodTypeDetail gim_foodTypeDetail { get; set; }
        public virtual gim_portionTypes gim_portionTypes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<gim_foodDetailDiet> gim_foodDetailDiet { get; set; }
    }
}
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
    
    public partial class tblRetracciones
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblRetracciones()
        {
            this.ConsultationFlexibility = new HashSet<ConsultationFlexibility>();
            this.tblConsultaXGrupoMuscularXRetracciones = new HashSet<tblConsultaXGrupoMuscularXRetracciones>();
        }
    
        public int intPKRetraccion { get; set; }
        public string strDescripcion { get; set; }
        public Nullable<int> intOrden { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ConsultationFlexibility> ConsultationFlexibility { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblConsultaXGrupoMuscularXRetracciones> tblConsultaXGrupoMuscularXRetracciones { get; set; }
    }
}

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
    
    public partial class tblEvaluacionPorSistemas
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblEvaluacionPorSistemas()
        {
            this.tblMovimientoEvaluacionPorSistemas = new HashSet<tblMovimientoEvaluacionPorSistemas>();
        }
    
        public int intPkEvaluacionPorSistema { get; set; }
        public string strDescripcion { get; set; }
        public int intEmpresa { get; set; }
        public bool bitActivo { get; set; }
        public string strUsuarioCreacion { get; set; }
        public System.DateTime datFechaCreacion { get; set; }
        public string strUsuarioModificacion { get; set; }
        public Nullable<System.DateTime> datFechaModificacion { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblMovimientoEvaluacionPorSistemas> tblMovimientoEvaluacionPorSistemas { get; set; }
    }
}

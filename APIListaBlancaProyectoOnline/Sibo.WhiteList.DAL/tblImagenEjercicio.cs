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
    
    public partial class tblImagenEjercicio
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblImagenEjercicio()
        {
            this.tblEjercicios = new HashSet<tblEjercicios>();
        }
    
        public int intPKImagenEjercicio { get; set; }
        public string strDescripcion { get; set; }
        public Nullable<int> intEmpresa { get; set; }
        public int intPublico { get; set; }
        public string strTipoEjercicio { get; set; }
        public string strNombreArchivo { get; set; }
        public bool bitActivo { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblEjercicios> tblEjercicios { get; set; }
    }
}

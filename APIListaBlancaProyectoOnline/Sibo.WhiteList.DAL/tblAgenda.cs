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
    
    public partial class tblAgenda
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblAgenda()
        {
            this.tblAgendaProfesional = new HashSet<tblAgendaProfesional>();
        }
    
        public int intPkAgenda { get; set; }
        public System.TimeSpan timHoraInicial { get; set; }
        public System.TimeSpan timHoraFinal { get; set; }
        public int intIntervalo { get; set; }
        public System.DateTime datFechaInicialVigencia { get; set; }
        public System.DateTime datFechaFinalVigencia { get; set; }
        public bool bitLunes { get; set; }
        public bool bitMartes { get; set; }
        public bool bitMiercoles { get; set; }
        public bool bitJueves { get; set; }
        public bool bitViernes { get; set; }
        public bool bitSabado { get; set; }
        public bool bitDomingo { get; set; }
        public int intEmpresa { get; set; }
        public bool bitActivo { get; set; }
        public string strUsuarioCreacion { get; set; }
        public System.DateTime datFechaCreacion { get; set; }
        public string strUsuarioModificacion { get; set; }
        public Nullable<System.DateTime> datFechaModificacion { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblAgendaProfesional> tblAgendaProfesional { get; set; }
    }
}

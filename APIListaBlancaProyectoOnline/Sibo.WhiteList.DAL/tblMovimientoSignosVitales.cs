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
    
    public partial class tblMovimientoSignosVitales
    {
        public int intPkMovimientoSignoVital { get; set; }
        public int intFkConsulta { get; set; }
        public int intFkSignoVital { get; set; }
        public int intValor { get; set; }
        public int intEmpresa { get; set; }
        public string strUsuarioCreacion { get; set; }
        public System.DateTime datFechaCreacion { get; set; }
        public string strUsuarioModificacion { get; set; }
        public Nullable<System.DateTime> datFechaModificacion { get; set; }
    
        public virtual tblConsulta tblConsulta { get; set; }
        public virtual tblSignosVitales tblSignosVitales { get; set; }
    }
}

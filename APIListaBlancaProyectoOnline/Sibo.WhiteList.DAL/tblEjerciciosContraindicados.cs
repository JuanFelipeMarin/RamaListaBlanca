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
    
    public partial class tblEjerciciosContraindicados
    {
        public int intPkEjerciciosContraindicados { get; set; }
        public int intFkConsulta { get; set; }
        public int intFkEjercicio { get; set; }
        public string strNota { get; set; }
        public int intEmpresa { get; set; }
        public string strUsuarioCreacion { get; set; }
        public System.DateTime datFechaCreacion { get; set; }
        public string strUsuarioModificacion { get; set; }
        public Nullable<System.DateTime> datFechaModificacion { get; set; }
    
        public virtual tblEjercicios tblEjercicios { get; set; }
    }
}

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
    
    public partial class tblConsultaXArticulaciones
    {
        public int intPKConsultaXArticulaciones { get; set; }
        public int intFKConsulta { get; set; }
        public int intFKArticulacion { get; set; }
        public string strObservaciones { get; set; }
        public bool bitLimitacion { get; set; }
    
        public virtual tblArticulaciones tblArticulaciones { get; set; }
        public virtual tblConsulta tblConsulta { get; set; }
    }
}
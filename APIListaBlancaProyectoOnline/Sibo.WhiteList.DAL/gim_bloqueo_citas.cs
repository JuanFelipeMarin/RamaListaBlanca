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
    
    public partial class gim_bloqueo_citas
    {
        public int id { get; set; }
        public int idcita { get; set; }
        public int cdgimnasio { get; set; }
        public string identidicacion { get; set; }
        public bool bitbloqueardiasdef { get; set; }
        public Nullable<System.DateTime> fecha_bloqueo { get; set; }
        public int dias_bloqueo { get; set; }
        public int especialidad { get; set; }
        public bool estado { get; set; }
        public Nullable<System.DateTime> fecha { get; set; }
    }
}

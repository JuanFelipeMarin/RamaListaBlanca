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
    
    public partial class gim_reservar_spinning
    {
        public int res_numero { get; set; }
        public Nullable<System.DateTime> res_fecha { get; set; }
        public Nullable<System.DateTime> res_hora { get; set; }
        public Nullable<double> res_identifi { get; set; }
        public Nullable<int> res_plan { get; set; }
        public string res_tipo_plan { get; set; }
        public Nullable<int> res_tiq_disponibles { get; set; }
        public bool res_anulada { get; set; }
        public Nullable<double> res_usuario { get; set; }
        public bool res_ingreso { get; set; }
        public Nullable<int> res_bicicleta { get; set; }
        public Nullable<int> res_fkintClase { get; set; }
        public Nullable<int> res_sede { get; set; }
        public int res_modificado { get; set; }
        public Nullable<int> res_intfkSucursal { get; set; }
        public Nullable<bool> bool_modificado_replica { get; set; }
        public Nullable<int> res_megas { get; set; }
    }
}
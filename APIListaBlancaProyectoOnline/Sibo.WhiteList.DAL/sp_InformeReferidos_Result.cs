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
    
    public partial class sp_InformeReferidos_Result
    {
        public string cli_identifi { get; set; }
        public string Correo { get; set; }
        public string Nombre { get; set; }
        public string ref_Nombre_Referido { get; set; }
        public bool ref_Estado { get; set; }
        public System.DateTime ref_Fecha_Referido { get; set; }
        public string ref_Correo_Referido { get; set; }
        public string ref_Codigo_Referido { get; set; }
        public Nullable<System.DateTime> cli_fecha_ingreso { get; set; }
    }
}
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
    
    public partial class sp_ListClassessAttendance_Result
    {
        public string cli_identifi { get; set; }
        public int cdreserva { get; set; }
        public int cdclase { get; set; }
        public int cdhorario_clase { get; set; }
        public string nombre_clase { get; set; }
        public System.DateTime fecha_clase { get; set; }
        public string dia { get; set; }
        public System.DateTime hora { get; set; }
        public System.DateTime hora_fin { get; set; }
        public Nullable<int> idClassAttendance { get; set; }
        public Nullable<System.DateTime> inicioAtencion { get; set; }
        public Nullable<System.DateTime> finAtencion { get; set; }
    }
}

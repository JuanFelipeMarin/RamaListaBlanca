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
    
    public partial class spConsultarHistorialCitasFiltro_Result
    {
        public int intPkCita { get; set; }
        public int intIdSucursal { get; set; }
        public System.DateTime datFechaCita { get; set; }
        public System.TimeSpan timHoraInicialCita { get; set; }
        public System.TimeSpan timHoraFinalCita { get; set; }
        public bool bitAtendida { get; set; }
        public bool bitCancelada { get; set; }
        public string strIdentificacionPaciente { get; set; }
        public string cli_nombres { get; set; }
        public string cli_primer_apellido { get; set; }
        public string cli_segundo_apellido { get; set; }
        public string strIdentificacionEmpleado { get; set; }
        public string emp_nombre { get; set; }
        public string emp_primer_apellido { get; set; }
        public string emp_segundo_apellido { get; set; }
        public Nullable<int> intPkEspecialidad { get; set; }
        public string strDescripcion { get; set; }
        public string Sucursal { get; set; }
        public string CORREO { get; set; }
        public string CELULAR { get; set; }
    }
}

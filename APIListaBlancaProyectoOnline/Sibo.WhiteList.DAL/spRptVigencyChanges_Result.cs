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
    
    public partial class spRptVigencyChanges_Result
    {
        public string Tipo { get; set; }
        public string TipoFecha { get; set; }
        public int cambio_numero_factura { get; set; }
        public Nullable<System.DateTime> cambio_fecha_actual { get; set; }
        public Nullable<System.DateTime> cambio_fecha_cambio { get; set; }
        public Nullable<System.DateTime> cambio_fecha_anterior { get; set; }
        public string cambio_descripcion { get; set; }
        public Nullable<double> cambio_usuario { get; set; }
        public string Empleado { get; set; }
        public string Cliente { get; set; }
        public int suc_intpkIdentificacion { get; set; }
        public string suc_strNombre { get; set; }
    }
}

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
    
    public partial class spDetalleCortesia_Result
    {
        public int idAbonado { get; set; }
        public double numAbonado { get; set; }
        public string identificacion { get; set; }
        public Nullable<int> trasladoPlancant { get; set; }
        public int idSucursalPlan { get; set; }
        public string nombre_cliente { get; set; }
        public Nullable<bool> grupof { get; set; }
        public string grupo { get; set; }
        public string subgrupo { get; set; }
        public decimal subgrupodescuento { get; set; }
        public string nombreSucursalPlan { get; set; }
        public string nombreSucursalVende { get; set; }
        public Nullable<int> idPlan { get; set; }
        public string nombrePlan { get; set; }
        public string fechaGeneracion { get; set; }
        public string fechaInicio { get; set; }
        public string fechaFin { get; set; }
        public Nullable<double> subTotal { get; set; }
        public Nullable<double> iva { get; set; }
        public Nullable<double> total { get; set; }
        public Nullable<double> idComercial { get; set; }
        public Nullable<bool> bitBono { get; set; }
        public Nullable<double> numFactura { get; set; }
        public Nullable<int> numResolucion { get; set; }
        public string observaciones { get; set; }
        public string usuario { get; set; }
        public string empresa { get; set; }
        public int cantidad { get; set; }
        public Nullable<bool> bitAvisado { get; set; }
        public Nullable<bool> bitImpreso { get; set; }
        public Nullable<bool> bitAnulado { get; set; }
        public Nullable<bool> bitFechaIni { get; set; }
        public Nullable<bool> bitFechaVen { get; set; }
        public string factFechaAnu { get; set; }
        public Nullable<int> Congelacion { get; set; }
        public string tiqDiasRestantes { get; set; }
        public int intTiquetesEntradas { get; set; }
        public int intTiquetesClases { get; set; }
    }
}

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
    
    public partial class gim_cobro_factura
    {
        public int Concecutivo_cobro { get; set; }
        public int nro_cobro_factura { get; set; }
        public int numero_fact_cobro { get; set; }
        public Nullable<double> cobro_valor { get; set; }
        public Nullable<int> cantidad_venta { get; set; }
        public Nullable<double> Iva_venta { get; set; }
        public int cobro_sucursal { get; set; }
        public int cobro_modificado { get; set; }
        public Nullable<int> cobro_sede { get; set; }
        public Nullable<bool> bool_modificado_replica { get; set; }
        public int cob_fkdia_codigo { get; set; }
        public string bits_replica { get; set; }
        public int cdgimnasio { get; set; }
        public Nullable<bool> cobro_bitIsAdministration { get; set; }
        public string MesPeriodoCuotaAdministracion { get; set; }
        public Nullable<int> PeriodoCuotaAdministracion { get; set; }
        public Nullable<System.DateTime> FechaRegistroCuotaAdministacion { get; set; }
    }
}
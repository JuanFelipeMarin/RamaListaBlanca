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
    
    public partial class spCobrosAdicionales_Result
    {
        public int nro_cobro { get; set; }
        public string detalle_cobro { get; set; }
        public Nullable<int> por_iva { get; set; }
        public Nullable<int> cant_inicial { get; set; }
        public Nullable<int> cant_vend { get; set; }
        public Nullable<int> cant_final { get; set; }
        public bool sin_valor { get; set; }
        public bool sin_cantidad { get; set; }
        public string codigo_barras { get; set; }
        public Nullable<double> @base { get; set; }
        public Nullable<double> valor_cobro { get; set; }
        public bool locker { get; set; }
        public Nullable<int> dias_locker { get; set; }
        public int cobro_adi_modificado { get; set; }
        public string pla_cod_cuenta { get; set; }
        public int cobro_adi_sucursal { get; set; }
        public Nullable<bool> bool_modificado_replica { get; set; }
        public Nullable<bool> cobro_adi_boolesquemapuntos { get; set; }
        public int cdgimnasio { get; set; }
        public Nullable<bool> cobro_adi_bitestado { get; set; }
        public string bits_replica { get; set; }
        public Nullable<bool> cobro_adi_bitentrenamiento { get; set; }
        public string cobro_adi_codbar { get; set; }
        public Nullable<double> cobro_adi_valorneto { get; set; }
        public string codigo_producto_interfaz_contable { get; set; }
        public string cuenta_contable { get; set; }
        public Nullable<int> clasificacion_cobro { get; set; }
        public bool bitReactivation { get; set; }
        public Nullable<bool> bitApportionmentRate { get; set; }
        public Nullable<bool> CashingsHealthCare { get; set; }
        public string Centro_de_costos_1 { get; set; }
        public string Centro_de_costos_2 { get; set; }
        public string Centro_de_costos_3 { get; set; }
        public string Clave_de_Productos_y_Servicios { get; set; }
        public string Clasificacion { get; set; }
    }
}

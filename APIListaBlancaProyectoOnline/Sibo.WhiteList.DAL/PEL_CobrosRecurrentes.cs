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
    
    public partial class PEL_CobrosRecurrentes
    {
        public string FK_strTokenUtilizado { get; set; }
        public string cr_strIdentificacion { get; set; }
        public int cr_intIdPlan { get; set; }
        public int cr_intIdSucursal { get; set; }
        public int cr_intReintentos { get; set; }
        public bool cr_bitEstado { get; set; }
        public System.DateTime cr_dtmFechaUltimoReintento { get; set; }
        public System.DateTime cr_dtmFechaSiguientePago { get; set; }
        public int cdgimnasio { get; set; }
        public int cr_Id { get; set; }
        public Nullable<int> cr_PaymentGatewayId { get; set; }
        public System.DateTime FechaDeCreacion { get; set; }
        public Nullable<System.DateTime> Fecha_Registro_Actualizacion_Reintentos { get; set; }
    
        public virtual PEL_Tokens PEL_Tokens { get; set; }
    }
}
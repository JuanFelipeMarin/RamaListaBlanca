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
    
    public partial class gim_BeneficiariosEmpresaCliente
    {
        public int cdgimnasio { get; set; }
        public string ben_strNitEmpresa { get; set; }
        public string ben_strIdentificacionBeneficiario { get; set; }
        public string ben_strIdentificacionEmpleado { get; set; }
        public int ben_intSucursal { get; set; }
        public System.DateTime ben_dtmFechaRegistro { get; set; }
        public bool ben_Seleccionado { get; set; }
        public int ben_modificado { get; set; }
        public Nullable<int> ben_intPlan { get; set; }
    }
}
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
    
    public partial class PEL_CobroCuenta
    {
        public string cc_strReferencia { get; set; }
        public int Id_cuenta { get; set; }
        public int Id_CobroRecDebito { get; set; }
        public int Id_Estado { get; set; }
        public bool cc_bitUsada { get; set; }
        public int cdgimnasio { get; set; }
        public string cc_descripcion { get; set; }
        public string cc_Identificador { get; set; }
        public string cc_LlaveTransaccional { get; set; }
        public string cc_LlaveMD5 { get; set; }
        public int cc_intId { get; set; }
        public string cc_URL { get; set; }
        public string cc_checksum { get; set; }
        public Nullable<decimal> cc_valor { get; set; }
        public Nullable<System.DateTime> cc_fecha { get; set; }
    
        public virtual PEL_CobroRecuDebito PEL_CobroRecuDebito { get; set; }
    }
}

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
    
    public partial class tblReplicaHuellas
    {
        public int intPkId { get; set; }
        public int intIdHuella_Tarjeta { get; set; }
        public string idPersona { get; set; }
        public string ipAddress { get; set; }
        public int idEmpresa { get; set; }
        public System.DateTime dtmfechaReplica { get; set; }
        public Nullable<bool> bitDelete { get; set; }
        public Nullable<bool> bitHuella { get; set; }
        public Nullable<bool> bitTarje_Pin { get; set; }
    }
}
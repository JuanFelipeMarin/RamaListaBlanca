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
    
    public partial class InvoiceXML
    {
        public int InvoiceID { get; set; }
        public int BranchID { get; set; }
        public int DianID { get; set; }
        public int gymId { get; set; }
        public string XMLText { get; set; }
        public Nullable<bool> Modified { get; set; }
        public string bits_replica { get; set; }
        public string CreationUser { get; set; }
        public System.DateTime CreationDate { get; set; }
    
        public virtual gim_planes_usuario gim_planes_usuario { get; set; }
    }
}
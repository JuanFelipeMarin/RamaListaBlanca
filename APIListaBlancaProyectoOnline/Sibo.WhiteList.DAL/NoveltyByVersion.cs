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
    
    public partial class NoveltyByVersion
    {
        public int id { get; set; }
        public int versionId { get; set; }
        public string noveltyType { get; set; }
        public string title { get; set; }
        public string noveltyDescription { get; set; }
        public bool bitState { get; set; }
        public bool bitDelete { get; set; }
        public int intOrder { get; set; }
        public System.DateTime creationDate { get; set; }
        public string creationUser { get; set; }
        public Nullable<System.DateTime> modifiedDate { get; set; }
        public string modifiedUser { get; set; }
    
        public virtual tblVersion tblVersion { get; set; }
    }
}

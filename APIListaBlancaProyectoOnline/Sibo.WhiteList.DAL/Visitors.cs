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
    
    public partial class Visitors
    {
        public int vis_intPK { get; set; }
        public string vis_strVisitorId { get; set; }
        public int vis_intTypeId { get; set; }
        public string vis_strName { get; set; }
        public string vis_strFirstLastName { get; set; }
        public string vis_strSecondLastName { get; set; }
        public System.DateTime vis_dtmDateBorn { get; set; }
        public System.DateTime vis_dtmRegisterDate { get; set; }
        public string vis_strPhone { get; set; }
        public string vis_strAddress { get; set; }
        public Nullable<int> vis_intGenre { get; set; }
        public string vis_strEmail { get; set; }
        public byte[] vis_imgPhoto { get; set; }
        public Nullable<int> vis_intEPS { get; set; }
        public Nullable<int> vis_intBranch { get; set; }
        public Nullable<bool> vis_EntryFingerprint { get; set; }
        public string vis_CreatedUser { get; set; }
        public Nullable<System.DateTime> vis_ModifiedDate { get; set; }
        public string vis_ModifiedUser { get; set; }
        public string bits_replica { get; set; }
        public int cdgimnasio { get; set; }
        public bool vis_bitModified { get; set; }
        public bool bitCortesia { get; set; }
        public bool bitCarnetCOVID19 { get; set; }
        public byte[] imgCarnetCOVID19 { get; set; }
        public Nullable<int> Visit_intPais { get; set; }
    }
}
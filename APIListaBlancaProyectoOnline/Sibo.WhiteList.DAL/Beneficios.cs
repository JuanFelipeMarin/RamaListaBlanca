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
    
    public partial class Beneficios
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public Nullable<int> cdgimnasio { get; set; }
        public Nullable<bool> aplicaSede { get; set; }
        public Nullable<bool> estado { get; set; }
        public Nullable<bool> aplicaPlan { get; set; }
        public string ben_strCreatedUser { get; set; }
        public Nullable<System.DateTime> ben_dtmCreatedDate { get; set; }
        public string ben_strModifiedUser { get; set; }
        public Nullable<System.DateTime> ben_dtmModifiedDate { get; set; }
    }
}
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
    
    public partial class SalesConsolidatedConcepts
    {
        public int idConcept { get; set; }
        public string concept { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public Nullable<int> order { get; set; }
        public Nullable<int> idGoal { get; set; }
        public Nullable<bool> detailByPlan { get; set; }
        public Nullable<bool> globalAffiliate { get; set; }
        public Nullable<int> idDetail { get; set; }
    }
}
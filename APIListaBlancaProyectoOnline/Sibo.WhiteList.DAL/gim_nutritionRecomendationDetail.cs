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
    
    public partial class gim_nutritionRecomendationDetail
    {
        public int PK_nutritionRecomendationDetailId { get; set; }
        public int cdgimnasio { get; set; }
        public int FK_feedingPlanId { get; set; }
        public int FK_nutritionRecomendationId { get; set; }
        public string description { get; set; }
    
        public virtual gim_feedingPlan gim_feedingPlan { get; set; }
        public virtual gim_nutritionRecomendations gim_nutritionRecomendations { get; set; }
    }
}

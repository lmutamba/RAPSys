//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RapModel.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class T_HouseholdCultureLastSeason
    {
        public int HouseholdCultureLastSeasonId { get; set; }
        public int HouseholdCultureId { get; set; }
        public decimal HarvestLastSeason { get; set; }
        public int HarvestUOM { get; set; }
        public decimal SoldLastSeason { get; set; }
        public int SoldUOM { get; set; }
        public decimal Revenue { get; set; }
        public int RevenueUOM { get; set; }
        public int PaidWorkersQty { get; set; }
        public System.DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
    
        public virtual T_HouseholdCulture T_HouseholdCulture { get; set; }
        public virtual T_UOM T_UOM { get; set; }
        public virtual T_UOM T_UOM1 { get; set; }
        public virtual T_UOM T_UOM2 { get; set; }
    }
}
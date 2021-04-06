//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RAPSys.Models.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class T_HouseHoldCultureLastSeason
    {
        public int HouseholdCultureLastSeasonId { get; set; }
        public int HouseholdCultureId { get; set; }
        public Nullable<decimal> HarvestLastSeason { get; set; }
        public Nullable<int> HarvestUOM { get; set; }
        public Nullable<decimal> SoldLastSeason { get; set; }
        public Nullable<int> SoldUOM { get; set; }
        public Nullable<decimal> Revenue { get; set; }
        public Nullable<int> RevenueUOM { get; set; }
        public Nullable<int> PaidWorkersQty { get; set; }
        public System.DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
    
        public virtual T_HouseHoldCulture T_HouseHoldCulture { get; set; }
        public virtual T_UOM T_UOM { get; set; }
        public virtual T_UOM T_UOM1 { get; set; }
        public virtual T_UOM T_UOM2 { get; set; }
    }
}

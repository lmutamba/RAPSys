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
    
    public partial class T_HouseHoldRevenue
    {
        public int HouseHoldRevenueId { get; set; }
        public int HouseHoldId { get; set; }
        public int RevenueSource { get; set; }
        public decimal RevenueAmount { get; set; }
        public int MonthsPerYear { get; set; }
        public System.DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
    
        public virtual T_HouseHold T_HouseHold { get; set; }
        public virtual T_List T_List { get; set; }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RapModel.DBModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class T_HouseHoldCultureLastMonths
    {
        public int HouseHoldCultureLastMonthsId { get; set; }
        public int HouseholdCultureId { get; set; }
        public int CultureActionType { get; set; }
        public System.DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
    
        public virtual T_HouseholdCulture T_HouseholdCulture { get; set; }
        public virtual T_List T_List { get; set; }
    }
}

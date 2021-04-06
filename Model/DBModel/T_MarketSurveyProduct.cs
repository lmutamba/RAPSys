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
    
    public partial class T_MarketSurveyProduct
    {
        public int MarketSurveyProductId { get; set; }
        public int Market { get; set; }
        public System.DateTime SurveyDate { get; set; }
        public int ProductId { get; set; }
        public int UOMId { get; set; }
        public int UOMQty { get; set; }
        public decimal Weight { get; set; }
        public decimal MarketPrice { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }
        public Nullable<decimal> WeightPrice { get; set; }
        public string Comments { get; set; }
        public System.DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
    
        public virtual T_List T_List { get; set; }
        public virtual T_Product T_Product { get; set; }
        public virtual T_UOM T_UOM { get; set; }
    }
}

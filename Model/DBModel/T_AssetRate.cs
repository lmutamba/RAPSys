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
    
    public partial class T_AssetRate
    {
        public int AssetRateId { get; set; }
        public int ProductId { get; set; }
        public int Year { get; set; }
        public System.DateTime CalculationDate { get; set; }
        public System.DateTime SubmissionDate { get; set; }
        public string Item { get; set; }
        public int UOMId { get; set; }
        public string SubItem { get; set; }
        public decimal MarketValue { get; set; }
        public int PriceUOM { get; set; }
        public Nullable<decimal> ProductionPerHa { get; set; }
        public Nullable<decimal> SecondaryCulture { get; set; }
        public decimal RatePerM2 { get; set; }
        public int ExchangeRateId { get; set; }
        public Nullable<int> PlantsPerha { get; set; }
        public Nullable<decimal> ProdPerMatureTree { get; set; }
        public Nullable<int> YearsToMaturity { get; set; }
        public decimal MarketValueOfCrop { get; set; }
        public decimal LostRevenuePerTree { get; set; }
        public Nullable<decimal> CostOfSeeding { get; set; }
        public decimal CostofLandPreparation { get; set; }
        public int SubItemUnit { get; set; }
        public decimal SubItemCost { get; set; }
        public string NotesOnCalculations { get; set; }
        public string Comments { get; set; }
        public System.DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
    
        public virtual T_ExchangeRate T_ExchangeRate { get; set; }
        public virtual T_Product T_Product { get; set; }
        public virtual T_UOM T_UOM { get; set; }
        public virtual T_UOM T_UOM1 { get; set; }
    }
}

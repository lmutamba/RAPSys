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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class RAPSystemEntities : DbContext
    {
        public RAPSystemEntities()
            : base("name=RAPSystemEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<T_Approval> T_Approval { get; set; }
        public virtual DbSet<T_Approver> T_Approver { get; set; }
        public virtual DbSet<T_AssetRate> T_AssetRate { get; set; }
        public virtual DbSet<T_AssetType> T_AssetType { get; set; }
        public virtual DbSet<T_Attachment> T_Attachment { get; set; }
        public virtual DbSet<T_CultureDiversity> T_CultureDiversity { get; set; }
        public virtual DbSet<T_CultureTool> T_CultureTool { get; set; }
        public virtual DbSet<T_Delegation> T_Delegation { get; set; }
        public virtual DbSet<T_Department> T_Department { get; set; }
        public virtual DbSet<T_EconomicActivity> T_EconomicActivity { get; set; }
        public virtual DbSet<T_Employee> T_Employee { get; set; }
        public virtual DbSet<T_EmployeeRole> T_EmployeeRole { get; set; }
        public virtual DbSet<T_ExchangeRate> T_ExchangeRate { get; set; }
        public virtual DbSet<T_Good> T_Good { get; set; }
        public virtual DbSet<T_HouseHold> T_HouseHold { get; set; }
        public virtual DbSet<T_HouseholdAttachment> T_HouseholdAttachment { get; set; }
        public virtual DbSet<T_HouseholdCulture> T_HouseholdCulture { get; set; }
        public virtual DbSet<T_HouseHoldCultureLastMonths> T_HouseHoldCultureLastMonths { get; set; }
        public virtual DbSet<T_HouseholdCultureLastSeason> T_HouseholdCultureLastSeason { get; set; }
        public virtual DbSet<T_HouseHoldCultureTool> T_HouseHoldCultureTool { get; set; }
        public virtual DbSet<T_HouseHoldGood> T_HouseHoldGood { get; set; }
        public virtual DbSet<T_HouseHoldMembers> T_HouseHoldMembers { get; set; }
        public virtual DbSet<T_HouseHoldProperty> T_HouseHoldProperty { get; set; }
        public virtual DbSet<T_HouseholdResidence> T_HouseholdResidence { get; set; }
        public virtual DbSet<T_HouseHoldRevenue> T_HouseHoldRevenue { get; set; }
        public virtual DbSet<T_LAC> T_LAC { get; set; }
        public virtual DbSet<T_LACRequest> T_LACRequest { get; set; }
        public virtual DbSet<T_Land> T_Land { get; set; }
        public virtual DbSet<T_LandVillage> T_LandVillage { get; set; }
        public virtual DbSet<T_List> T_List { get; set; }
        public virtual DbSet<T_Location> T_Location { get; set; }
        public virtual DbSet<T_MarketSurveyProduct> T_MarketSurveyProduct { get; set; }
        public virtual DbSet<T_PAP> T_PAP { get; set; }
        public virtual DbSet<T_PAPAttachment> T_PAPAttachment { get; set; }
        public virtual DbSet<T_PAPEconomicActivity> T_PAPEconomicActivity { get; set; }
        public virtual DbSet<T_PAPLAC> T_PAPLAC { get; set; }
        public virtual DbSet<T_PAPLACAttachment> T_PAPLACAttachment { get; set; }
        public virtual DbSet<T_PAPResidence> T_PAPResidence { get; set; }
        public virtual DbSet<T_Parameter> T_Parameter { get; set; }
        public virtual DbSet<T_Person> T_Person { get; set; }
        public virtual DbSet<T_Point> T_Point { get; set; }
        public virtual DbSet<T_Product> T_Product { get; set; }
        public virtual DbSet<T_Property> T_Property { get; set; }
        public virtual DbSet<T_PropertyAttachment> T_PropertyAttachment { get; set; }
        public virtual DbSet<T_Region> T_Region { get; set; }
        public virtual DbSet<T_Request> T_Request { get; set; }
        public virtual DbSet<T_RequestAttachment> T_RequestAttachment { get; set; }
        public virtual DbSet<T_Residence> T_Residence { get; set; }
        public virtual DbSet<T_UOM> T_UOM { get; set; }
        public virtual DbSet<T_Village> T_Village { get; set; }
        public virtual DbSet<T_WorkFlow> T_WorkFlow { get; set; }
    }
}

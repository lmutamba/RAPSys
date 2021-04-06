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
    
    public partial class T_HouseHoldMembers
    {
        public int HouseholdMemberId { get; set; }
        public int PersonId { get; set; }
        public int HouseholdId { get; set; }
        public string MaritalStatus { get; set; }
        public int Relationship { get; set; }
        public bool IsStudent { get; set; }
        public int EducationLevel { get; set; }
        public int FrenchLevel { get; set; }
        public string Picture { get; set; }
        public int ResidenceId { get; set; }
        public int Activity1 { get; set; }
        public Nullable<int> Activity2 { get; set; }
        public int Competency1 { get; set; }
        public Nullable<int> Competency2 { get; set; }
        public string Comments { get; set; }
        public System.DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
    
        public virtual T_HouseHold T_HouseHold { get; set; }
        public virtual T_List T_List { get; set; }
        public virtual T_List T_List1 { get; set; }
        public virtual T_List T_List2 { get; set; }
        public virtual T_List T_List3 { get; set; }
        public virtual T_List T_List4 { get; set; }
        public virtual T_List T_List5 { get; set; }
        public virtual T_List T_List6 { get; set; }
        public virtual T_Person T_Person { get; set; }
        public virtual T_Residence T_Residence { get; set; }
    }
}

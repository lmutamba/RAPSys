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
    
    public partial class T_HouseholdResidence
    {
        public int HouseholdResidenceId { get; set; }
        public int HouseholdId { get; set; }
        public int ResidenceId { get; set; }
        public bool IsOwner { get; set; }
        public decimal Rent { get; set; }
        public System.DateTime ArrivalDate { get; set; }
        public int ArrivalReason { get; set; }
        public int ToiletType { get; set; }
        public bool IsPermanent { get; set; }
        public System.DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
    
        public virtual T_HouseHold T_HouseHold { get; set; }
        public virtual T_List T_List { get; set; }
        public virtual T_List T_List1 { get; set; }
        public virtual T_Residence T_Residence { get; set; }
    }
}
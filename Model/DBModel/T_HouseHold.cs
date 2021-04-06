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
    
    public partial class T_HouseHold
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_HouseHold()
        {
            this.T_HouseholdAttachment = new HashSet<T_HouseholdAttachment>();
            this.T_HouseholdCulture = new HashSet<T_HouseholdCulture>();
            this.T_HouseHoldGood = new HashSet<T_HouseHoldGood>();
            this.T_HouseHoldProperty = new HashSet<T_HouseHoldProperty>();
            this.T_HouseholdResidence = new HashSet<T_HouseholdResidence>();
            this.T_HouseHoldRevenue = new HashSet<T_HouseHoldRevenue>();
            this.T_PAP = new HashSet<T_PAP>();
        }
    
        public int HouseHoldId { get; set; }
        public int HouseHoldResponsible { get; set; }
        public int HouseHoldNumber { get; set; }
        public bool PreviouslyCompensated { get; set; }
        public string CompensationUse { get; set; }
        public int ResidenceId { get; set; }
        public Nullable<decimal> LastWeekExpense { get; set; }
        public System.DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
    
        public virtual T_Person T_Person { get; set; }
        public virtual T_Residence T_Residence { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_HouseholdAttachment> T_HouseholdAttachment { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_HouseholdCulture> T_HouseholdCulture { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_HouseHoldGood> T_HouseHoldGood { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_HouseHoldProperty> T_HouseHoldProperty { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_HouseholdResidence> T_HouseholdResidence { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_HouseHoldRevenue> T_HouseHoldRevenue { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_PAP> T_PAP { get; set; }
    }
}

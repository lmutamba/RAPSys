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
    
    public partial class T_Residence
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_Residence()
        {
            this.T_HouseHold = new HashSet<T_HouseHold>();
            this.T_HouseHoldMembers = new HashSet<T_HouseHoldMembers>();
            this.T_HouseholdResidence = new HashSet<T_HouseholdResidence>();
            this.T_PAPResidence = new HashSet<T_PAPResidence>();
        }
    
        public int ResidenceId { get; set; }
        public string ResidenceAddress { get; set; }
        public System.DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_HouseHold> T_HouseHold { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_HouseHoldMembers> T_HouseHoldMembers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_HouseholdResidence> T_HouseholdResidence { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_PAPResidence> T_PAPResidence { get; set; }
    }
}

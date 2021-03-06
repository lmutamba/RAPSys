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
    
    public partial class T_Person
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_Person()
        {
            this.T_Employee = new HashSet<T_Employee>();
            this.T_HouseHold = new HashSet<T_HouseHold>();
            this.T_HouseHoldMembers = new HashSet<T_HouseHoldMembers>();
            this.T_PAP = new HashSet<T_PAP>();
            this.T_PAP1 = new HashSet<T_PAP>();
            this.T_PersonRole = new HashSet<T_PersonRole>();
            this.T_Person1 = new HashSet<T_Person>();
            this.T_Person11 = new HashSet<T_Person>();
        }
    
        public int PersonId { get; set; }
        public string PersonType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string IdCard { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public string PlaceOfBirth { get; set; }
        public Nullable<int> Father { get; set; }
        public Nullable<int> Mother { get; set; }
        public Nullable<int> VulnerabilityTypeId { get; set; }
        public string VulnerabilityDetail { get; set; }
        public Nullable<int> Status { get; set; }
        public System.DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_Employee> T_Employee { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_HouseHold> T_HouseHold { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_HouseHoldMembers> T_HouseHoldMembers { get; set; }
        public virtual T_List T_List { get; set; }
        public virtual T_List T_List1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_PAP> T_PAP { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_PAP> T_PAP1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_PersonRole> T_PersonRole { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_Person> T_Person1 { get; set; }
        public virtual T_Person T_Person2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_Person> T_Person11 { get; set; }
        public virtual T_Person T_Person3 { get; set; }
    }
}

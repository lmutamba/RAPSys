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
    
    public partial class T_Land
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_Land()
        {
            this.T_HouseHoldProperty = new HashSet<T_HouseHoldProperty>();
            this.T_LAC = new HashSet<T_LAC>();
            this.T_LandVillage = new HashSet<T_LandVillage>();
            this.T_Point = new HashSet<T_Point>();
            this.T_Property = new HashSet<T_Property>();
        }
    
        public int LandId { get; set; }
        public int LandCategory { get; set; }
        public int LACRequestId { get; set; }
        public int RegionId { get; set; }
        public System.DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_HouseHoldProperty> T_HouseHoldProperty { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_LAC> T_LAC { get; set; }
        public virtual T_LACRequest T_LACRequest { get; set; }
        public virtual T_List T_List { get; set; }
        public virtual T_Region T_Region { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_LandVillage> T_LandVillage { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_Point> T_Point { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_Property> T_Property { get; set; }
    }
}

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
    
    public partial class T_UOM
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_UOM()
        {
            this.T_AssetRate = new HashSet<T_AssetRate>();
            this.T_AssetRate1 = new HashSet<T_AssetRate>();
            this.T_HouseHoldCompensation = new HashSet<T_HouseHoldCompensation>();
            this.T_HouseHoldCulture = new HashSet<T_HouseHoldCulture>();
            this.T_HouseHoldCultureLastSeason = new HashSet<T_HouseHoldCultureLastSeason>();
            this.T_HouseHoldCultureLastSeason1 = new HashSet<T_HouseHoldCultureLastSeason>();
            this.T_HouseHoldCultureLastSeason2 = new HashSet<T_HouseHoldCultureLastSeason>();
            this.T_HouseHoldGood = new HashSet<T_HouseHoldGood>();
            this.T_HouseHoldProperty = new HashSet<T_HouseHoldProperty>();
            this.T_HouseHoldProperty1 = new HashSet<T_HouseHoldProperty>();
            this.T_HouseHoldProperty2 = new HashSet<T_HouseHoldProperty>();
            this.T_LAC = new HashSet<T_LAC>();
            this.T_LAC1 = new HashSet<T_LAC>();
            this.T_LAC2 = new HashSet<T_LAC>();
            this.T_Land = new HashSet<T_Land>();
            this.T_MarketSurveyProduct = new HashSet<T_MarketSurveyProduct>();
            this.T_Property = new HashSet<T_Property>();
            this.T_Property1 = new HashSet<T_Property>();
            this.T_UOM1 = new HashSet<T_UOM>();
        }
    
        public int UOMId { get; set; }
        public string UOM { get; set; }
        public Nullable<int> UOMReference { get; set; }
        public decimal UOMConversion { get; set; }
        public int UOMType { get; set; }
        public System.DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_AssetRate> T_AssetRate { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_AssetRate> T_AssetRate1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_HouseHoldCompensation> T_HouseHoldCompensation { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_HouseHoldCulture> T_HouseHoldCulture { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_HouseHoldCultureLastSeason> T_HouseHoldCultureLastSeason { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_HouseHoldCultureLastSeason> T_HouseHoldCultureLastSeason1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_HouseHoldCultureLastSeason> T_HouseHoldCultureLastSeason2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_HouseHoldGood> T_HouseHoldGood { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_HouseHoldProperty> T_HouseHoldProperty { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_HouseHoldProperty> T_HouseHoldProperty1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_HouseHoldProperty> T_HouseHoldProperty2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_LAC> T_LAC { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_LAC> T_LAC1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_LAC> T_LAC2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_Land> T_Land { get; set; }
        public virtual T_List T_List { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_MarketSurveyProduct> T_MarketSurveyProduct { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_Property> T_Property { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_Property> T_Property1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_UOM> T_UOM1 { get; set; }
        public virtual T_UOM T_UOM2 { get; set; }
    }
}

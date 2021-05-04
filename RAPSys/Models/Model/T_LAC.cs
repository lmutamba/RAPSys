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
    
    public partial class T_LAC
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_LAC()
        {
            this.T_LandLAC = new HashSet<T_LandLAC>();
            this.T_PAPLAC = new HashSet<T_PAPLAC>();
            this.T_Property = new HashSet<T_Property>();
        }
    
        public int LACId { get; set; }
        public string LAC_ID { get; set; }
        public string LACName { get; set; }
        public int LACRequestId { get; set; }
        public int LACStatus { get; set; }
        public decimal Realcosts { get; set; }
        public int PAPs { get; set; }
        public Nullable<decimal> AuthorizedAreaSize { get; set; }
        public Nullable<int> AuthorizedAreaSizeUOM { get; set; }
        public Nullable<System.DateTime> AuthorizedDate { get; set; }
        public string AreaDescription { get; set; }
        public Nullable<decimal> AreaRequested { get; set; }
        public Nullable<int> AreaRequestedUOM { get; set; }
        public Nullable<decimal> AreaCompensed { get; set; }
        public Nullable<int> AreaCompensedUOM { get; set; }
        public Nullable<decimal> CostEstimate { get; set; }
        public string Comment { get; set; }
        public bool Locked { get; set; }
        public System.DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
    
        public virtual T_LACRequest T_LACRequest { get; set; }
        public virtual T_List T_List { get; set; }
        public virtual T_UOM T_UOM { get; set; }
        public virtual T_UOM T_UOM1 { get; set; }
        public virtual T_UOM T_UOM2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_LandLAC> T_LandLAC { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_PAPLAC> T_PAPLAC { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_Property> T_Property { get; set; }
    }
}

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
    
    public partial class T_EconomicActivity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_EconomicActivity()
        {
            this.T_PAPEconomicActivity = new HashSet<T_PAPEconomicActivity>();
        }
    
        public int EconomicActivityId { get; set; }
        public string EconomicActivityName { get; set; }
        public System.DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime Updated { get; set; }
        public string UpdateBy { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_PAPEconomicActivity> T_PAPEconomicActivity { get; set; }
    }
}

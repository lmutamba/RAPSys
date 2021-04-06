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
    
    public partial class T_LACRequest
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_LACRequest()
        {
            this.T_LAC = new HashSet<T_LAC>();
            this.T_Land = new HashSet<T_Land>();
        }
    
        public int LACRequestId { get; set; }
        public int Requestid { get; set; }
        public string ProjectName { get; set; }
        public int LocationId { get; set; }
        public string ProjectCostCode { get; set; }
        public string WorkDescription { get; set; }
        public System.DateTime AccessScheduledDate { get; set; }
        public bool IsUrgent { get; set; }
        public int ContactPerson { get; set; }
        public System.DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
    
        public virtual T_Employee T_Employee { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_LAC> T_LAC { get; set; }
        public virtual T_Location T_Location { get; set; }
        public virtual T_Request T_Request { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_Land> T_Land { get; set; }
    }
}

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
    
    public partial class T_PersonRole
    {
        public int PersonRoleId { get; set; }
        public int PersonId { get; set; }
        public int Role { get; set; }
        public Nullable<int> Status { get; set; }
        public System.DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
    
        public virtual T_List T_List { get; set; }
        public virtual T_List T_List1 { get; set; }
        public virtual T_Person T_Person { get; set; }
    }
}

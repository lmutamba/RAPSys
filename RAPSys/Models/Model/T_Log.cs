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
    
    public partial class T_Log
    {
        public long Log_ID { get; set; }
        public string Person_ID { get; set; }
        public string Action { get; set; }
        public string Table_Name { get; set; }
        public long Table_Reference_ID { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public System.DateTime LoggedDate { get; set; }
    }
}

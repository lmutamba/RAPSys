using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RapModel.ViewModel
{
    public class ApprovalViewModel
    {
        public int ApprovalId { get; set; }
        public int RequestId { get; set; }
        public int RequestType { get; set; }
        public string RequestTypeString { get; set; }
        public int RequestStatus { get; set; }
        public string RequestStatusString { get; set; }
        public DateTime RequestedDate { get; set; }
        public string RequestDateString { get; set; }
        public int Requestor { get; set; }
        public string RequestorName { get; set; }
        public int CurrentStep { get; set; }
        public string ProjectName { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public string ProjectCostCode { get; set; }
        public string WorkDescription { get; set; }
        public DateTime AccessScheduledDate { get; set; }
        public string AccessScheduleDateString { get; set; }
        public bool IsUrgent { get; set; }
        public int ContactPerson { get; set; }
        public string ContactPersonName { get; set; }
        public int Approver { get; set; }
        public string ApproverName { get; set; }
        public DateTime ActionDate { get; set; }
        public string ActionDateString { get; set; }
        public int ApprovalStatus { get; set; }
        public string ApprovalStatusString { get; set; }
        public bool IsApproved { get; set; }
        public string ApprovalComments { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
        public HttpPostedFileBase[] Attachments { get; set; }
        public string[] AttachmentsName { get; set; }
        public string AttachmentsList { get; set; }
    }
}
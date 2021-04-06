using System;
using System.Web;

namespace RapModel.ViewModel
{
    public class LandRequestViewModel
    {
        public int LACRequestId { get; set; }
        public string ProjectName { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public int RegionId { get; set; }
        public string RegionName { get; set; }
        public string ProjectCostCode { get; set; }
        public string WorkDescription { get; set; }
        public DateTime AccessScheduledDate { get; set; }
        public string AccessDateString { get; set; }
        public bool IsUrgent { get; set; }
        public int ContactPersonId { get; set; }
        public string ContactPerson { get; set; }
        public int RequestId { get; set; }
        public int RequestType { get; set; }
        public string RequestTypeName { get; set; }
        public int RequestStatusID { get; set; }
        public string RequestStatus { get; set; }
        public DateTime RequestedDate { get; set; }
        public int Requestor { get; set; }
        public string RequestorName { get; set; }
        public string RequestorDepartment { get; set; }
        public int CurrentStep { get; set; }
        public int LandID { get; set; }
        public int LandStatusId { get; set; }
        public string LandStatusString { get; set; }
        public decimal AreaSurface { get; set; }
        public int AreaSurfaceUOM { get; set; }
        public string AreaSurfaceUOMString { get; set; }
        public string AreaSurfaceString { get; set; }
        public DateTime AuthorizedDate { get; set; }
        public string AuthorizedDateString { get; set; }
        public DateTime GPSDate { get; set; }
        public string GPSDateString { get; set; }
        public string LandComment { get; set; }
        public decimal LandEasting { get; set; }
        public decimal LandNorthing { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
        public HttpPostedFileBase[] Attachments { get; set; }
        public string[] AttachmentsName { get; set; }
        public string AttachmentsList { get; set; }
        public PointViewModel[] LandRequestPoints { get; set; }
        public AttachmentViewModel[] AttachmentsDocuments { get; set; }
    }
}
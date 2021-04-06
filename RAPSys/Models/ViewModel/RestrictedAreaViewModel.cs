using System;

namespace RapModel.ViewModel
{
    public class RestrictedAreaViewModel
    {
        public int LandId { get; set; }
        public int LACRequestId { get; set; }
        public string LandName { get; set; }
        public int LandLacID { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public int RegionId { get; set; }
        public string RegionName { get; set; }
        public int ContactPersonId { get; set; }
        public string ContactPerson { get; set; }
        public DateTime RequestedDate { get; set; }
        public string RequestedDateString { get; set; }
        public int Requestor { get; set; }
        public string RequestorName { get; set; }
        public int LandCategory { get; set; }
        public string LandCategoryString { get; set; }
        public int LandStatusId { get; set; }
        public string LandStatusString { get; set; }
        public decimal AreaSurface { get; set; }
        public int AreaSurfaceUOM { get; set; }
        public string AreaSurfaceUOMString { get; set; }
        public string AreaSurfaceString { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public DateTime GPSDate { get; set; }
        public string GPSDateString { get; set; }
        public decimal LandEasting { get; set; }
        public decimal LandNorthing { get; set; }
        public string LandComment { get; set; }
        public int LacID { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
        public PointViewModel[] LandRequestPoints { get; set; }
        public VillageViewModel[] LandVillage { get; set; }
        public AttachmentViewModel[] AttachmentsDocuments { get; set; }
    }
}
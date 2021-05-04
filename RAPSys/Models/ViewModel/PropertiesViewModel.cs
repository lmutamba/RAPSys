﻿using System;
using System.Web;

namespace RapModel.ViewModel
{
    public class PropertiesViewModel
    {
        public int PropertyId { get; set; }
        public int Owner { get; set; }
        public bool IsOwner { get; set; }
        public int User { get; set; }
        public bool IsUser { get; set; }
        public string UserType { get; set; }
        public int? OwnerID { get; set; }
        public string OwnerFirstName { get; set; }
        public string OwnerLastName { get; set; }
        public string OwnerMiddleName { get; set; }
        public string OwnerFileNumber { get; set; }
        public int? OwnerPictureId { get; set; }
        public string OwnerPictureName { get; set; }
        public int? OwnerPrimaryResidenceId { get; set; }
        public string OwnerPrimaryResidenceName { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserMiddleName { get; set; }
        public string UserFileNumber { get; set; }
        public string UserPictureName { get; set; }
        public int? UserPrimaryResidenceId { get; set; }
        public string UserPrimaryResidenceName { get; set; }
        public int LandId { get; set; }
        public int VillageId { get; set; }
        public string VillageName { get; set; }
        public int LACId { get; set; }
        public string LACName { get; set; }
        public int Investigator { get; set; }
        public string InvestigatorCode { get; set; }
        public int InvestigatorPoint { get; set; }
        public string IPointName { get; set; }
        public decimal IPointLatitude { get; set; }
        public decimal IPointLongitude { get; set; }
        public decimal? IPointElevation { get; set; }
        public int? TopographerPoint { get; set; }
        public string TPointName { get; set; }
        public decimal? TPointLatitude { get; set; }
        public decimal? TPointLongitude { get; set; }
        public decimal? TPointElevation { get; set; }
        public string PictureName { get; set; }
        public string Picture { get; set; }
        public int PropertyType { get; set; }
        public string PropertyTypeName { get; set; }
        public string PropertyName { get; set; }
        public decimal? Surface { get; set; }
        public int? SurfaceUOM { get; set; }
        public string SurfaceUOMName { get; set; }
        public string SurfaceString { get; set; }
        public DateTime? CultureStartDate { get; set; }
        public string CultureStartDateString { get; set; }
        public int? CultureOccupationType { get; set; }
        public string CultureOccupationName { get; set; }
        public int? TreeMaturity { get; set; }
        public string TreeMaturityName { get; set; }
        public int? TreeQty { get; set; }
        public int? StructureType { get; set; }
        public string StructureTypeName { get; set; }
        public int? RoomQty { get; set; }
        public decimal? StructureLength { get; set; }
        public decimal? StructureWidth { get; set; }
        public int? RoofType { get; set; }
        public string RoofTypeName { get; set; }
        public int? WallType { get; set; }
        public string WallTypeName { get; set; }
        public int? SoilType { get; set; }
        public string SoilTypeName { get; set; }
        public int? StructureUOM { get; set; }
        public string StructureUOMString { get; set; }
        public int? StructureUsageID { get; set; }
        public string StructureUsageName { get; set; }
        public DateTime? MeasurementSignatureDate { get; set; }
        public string MeasurementSignatureDateString { get; set; }
        public int? TopographerId { get; set; }
        public string TopographerCode { get; set; }
        public DateTime TopographDate { get; set; }
        public string TopographDateString { get; set; }
        public string TrimbleCode { get; set; }
        public string TrimbleFile { get; set; }
        public DateTime? MeasurementDate { get; set; }
        public string MeasurementDateString { get; set; }
        public string CameraCode { get; set; }
        public string GPSDevice { get; set; }
        public DateTime InventoryDate { get; set; }
        public string InventoryDateString { get; set; }
        public string Comments { get; set; }
        public bool IsHabitable { get; set; }
        public bool HasOwner { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
        public string CulturePoints { get; set; }
    }
}

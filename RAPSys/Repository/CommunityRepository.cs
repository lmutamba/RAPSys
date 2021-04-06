using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Data.Entity;
using RapModel.ViewModel;
using RAPSys.Models.Model;
using System.Transactions;
using Newtonsoft.Json;
using System.Web.Security;

namespace Repository
{
    public class CommunityRepository
    {
        readonly HelpersRepository Helpers = new HelpersRepository();
        readonly RAPSystemEntities db = new RAPSystemEntities();
        private readonly string loggedUser;
        private const string folderRoot = HelpersRepository.folderRoot;

        public CommunityRepository()
        {
            loggedUser = Helpers.loggedUser;
        }

        #region LOAD DATA
        public IEnumerable<PAPViewModel> LoadPAP(int LacID)
        {
            var pap = db.T_PAPLAC.Where(p => p.LACId == LacID).ToList().Select(p => new PAPViewModel()
            {
                PAPLACId = p.PAPLACId,
                PAPId = p.PAPId,
                FirstName = p.T_PAP.T_Person.FirstName,
                LastName = p.T_PAP.T_Person.LastName,
                FileNumber = p.FileNumber,
                LACId = p.LACId,
                Picture = PictureLink(p.PAPId, "PAPLAC"),
                ResidenceID = p.T_PAP.T_PAPResidence.FirstOrDefault(r => r.IsPermanent).ResidenceId,
                ResidenceName = p.T_PAP.T_PAPResidence.FirstOrDefault(r => r.IsPermanent).T_Residence.ResidenceAddress,
                HouseHoldId = p.T_PAP.HouseHoldId
            });
            return pap;
        }

        public IEnumerable<PAPViewModel> LoadPAP()
        {
            var pap = db.T_PAP.ToList().Select(p => new PAPViewModel()
            {
                PAPId = p.PAPId,
                PersonId = p.T_Person.PersonId,
                FirstName = p.T_Person.FirstName,
                LastName = p.T_Person.LastName,
                MiddleName = p.T_Person.MiddleName,
                Mobile = p.T_Person.Mobile,
                Email = p.T_Person.Email,
                Gender = p.T_Person.Gender,
                DateOfBirth = p.T_Person.DateOfBirth,
                DateOfBirthString = p.T_Person.DateOfBirth?.ToString("dd/MMM/yyyy"),
                PlaceOfBirth = p.T_Person.PlaceOfBirth,
                Father = p.T_Person.Father,
                FatherName = p?.T_Person?.T_Person2?.FirstName + " " + p?.T_Person?.T_Person2?.LastName + " " + p?.T_Person?.T_Person2?.MiddleName,
                Mother = p.T_Person.Mother,
                MotherName = p?.T_Person?.T_Person3?.FirstName + " " + p?.T_Person?.T_Person3?.LastName + " " + p?.T_Person?.T_Person3?.MiddleName,
                Spouse = p.Spouse,
                SpouseName = p.T_Person1?.FirstName + " " + p.T_Person1?.LastName + " " + p.T_Person1?.MiddleName,
                VulnerabilityTypeId = p.T_Person.VulnerabilityTypeId ?? 0,
                VulnerabilityTypeName = (bool)p.T_Person.VulnerabilityTypeId.HasValue ? p.T_Person.T_List.ListValue : "",
                VulnerabilityDetail = p.T_Person.VulnerabilityDetail,
                IdCard = p.IdCard,
                Picture = PictureLink(p.PAPId, "PAP"),
                ResidenceID = p.T_PAPResidence.FirstOrDefault(pr => pr.IsPermanent).ResidenceId,
                ResidenceName = p.T_PAPResidence.FirstOrDefault(pr => pr.IsPermanent).T_Residence.ResidenceAddress,
                HouseHoldId = p.HouseHoldId
            });
            return pap;
        }

        public IEnumerable<PAPViewModel> LoadPAPLAC(int PapID)
        {
            var pap = db.T_PAPLAC.Where(p => p.PAPId == PapID).ToList().Select(p => new PAPViewModel()
            {
                PAPLACId = p.PAPLACId,
                PAPId = p.PAPId,
                FirstName = p.T_PAP.T_Person.FirstName,
                LastName = p.T_PAP.T_Person.LastName,
                FileNumber = p.FileNumber,
                LACId = p.LACId,
                LACName = p.T_LAC?.LACName,
                PaymentPreference = p.PaymentPreference?? default(int?),
                PaymentPreferenceName = p.T_List?.ListValue?? default,
                FormSubmissionDate = p.FormSubmissionDate?? default,
                FormSubmissionDateString = p.FormSubmissionDate?.ToString("dd/MMMM/yyyy"),
                PresurveyDate = p.PresurveyDate?? default,
                PresurveyDateString = p.PresurveyDate?.ToString("dd/MMMM/yyyy"),
                Presurveyor = p.Presurveyor?? default(int?),
                PresurveyorCode = p.T_Employee?.EmployeeCode,
                PresurveyorName = p.T_Employee?.T_Person.FirstName + " " + p.T_Employee?.T_Person.LastName + " " + p.T_Employee?.T_Person.MiddleName,
                SurveyDate = p.SurveyDate?? default,
                SurveyDateString = p.SurveyDate?.ToString("dd/MMMM/yyyy"),
                Surveyor = p.Surveyor ?? default(int?),
                SurveyorCode = p.T_Employee1?.EmployeeCode,
                SurveyorName = p.T_Employee1?.T_Person.FirstName + " " + p.T_Employee1?.T_Person.LastName + " " + p.T_Employee1?.T_Person.MiddleName,
                Comments = p.Comments,
                HouseHoldId = p.T_PAP.HouseHoldId
            });
            return pap;
        }

        public IEnumerable<PAPViewModel> LoadPAPLAC()
        {
            var plac = db.T_PAPLAC.Distinct().Select(p=>p.PAPId).ToList();
            var pap = db.T_PAPLAC.ToList().Select(p => new PAPViewModel()
            {
                PAPLACId = p.PAPLACId,
                PAPId = p.PAPId,
                PersonId = p.T_PAP.T_Person.PersonId,
                FirstName = p.T_PAP.T_Person.FirstName,
                LastName = p.T_PAP.T_Person.LastName,
                FileNumber = p.FileNumber,
                LACId = p.LACId,
                LACName = p.T_LAC?.LACName,
                Mobile = p.T_PAP.T_Person.Mobile,
                PaymentPreference = p.PaymentPreference ?? default(int?),
                PaymentPreferenceName = p.T_List?.ListValue ?? default,
                FormSubmissionDate = p.FormSubmissionDate ?? default,
                FormSubmissionDateString = p.FormSubmissionDate?.ToString("dd/MMMM/yyyy"),
                PresurveyDate = p.PresurveyDate ?? default,
                PresurveyDateString = p.PresurveyDate?.ToString("dd/MMMM/yyyy"),
                Presurveyor = p.Presurveyor ?? default(int?),
                PresurveyorCode = p.T_Employee?.EmployeeCode,
                PresurveyorName = p.T_Employee?.T_Person.FirstName + " " + p.T_Employee?.T_Person.LastName + " " + p.T_Employee?.T_Person.MiddleName,
                SurveyDate = p.SurveyDate ?? default,
                SurveyDateString = p.SurveyDate?.ToString("dd/MMMM/yyyy"),
                Surveyor = p.Surveyor ?? default(int?),
                SurveyorCode = p.T_Employee1?.EmployeeCode,
                SurveyorName = p.T_Employee1?.T_Person.FirstName + " " + p.T_Employee1?.T_Person.LastName + " " + p.T_Employee1?.T_Person.MiddleName,
                Comments = p.Comments,
                IdCard = p.T_PAP.IdCard,
                Picture = PictureLink(p.PAPId, "PAP"),
                PictureBase64 = p.T_PAP.Picture,
                PhotoID = int.Parse(p.T_PAP.Picture.Substring(p.T_PAP.Picture.LastIndexOf("\\") + 1).Split('.')[0]),
                ResidenceID = p.T_PAP.T_PAPResidence.FirstOrDefault(pr => pr.IsPermanent).ResidenceId,
                ResidenceName = p.T_PAP.T_PAPResidence.FirstOrDefault(pr => pr.IsPermanent).T_Residence.ResidenceAddress,
                HouseHoldId = p.T_PAP.HouseHoldId
            });

            var pap2 = db.T_PAP.Where(pp => !plac.Contains(pp.PAPId)).ToList().Select(p => new PAPViewModel()
            {
                PAPLACId = 0,
                PAPId = p.PAPId,
                PersonId = p.T_Person.PersonId,
                FirstName = p.T_Person.FirstName,
                LastName = p.T_Person.LastName,
                FileNumber = null,
                LACId = -1,
                LACName = "Unassign",
                Mobile = p.T_Person.Mobile,
                PaymentPreference = default,
                PaymentPreferenceName = default,
                FormSubmissionDate = default,
                FormSubmissionDateString = null,
                PresurveyDate = default,
                PresurveyDateString = null,
                Presurveyor = default,
                PresurveyorCode = null,
                PresurveyorName = null,
                SurveyDate = default,
                SurveyDateString = null,
                Surveyor = default,
                SurveyorCode = null,
                SurveyorName = null,
                Comments = null,
                IdCard = p.IdCard,
                Picture = PictureLink(p.PAPId, "PAP"),
                PictureBase64 = p.Picture,
                PhotoID = int.Parse(p.Picture.Substring(p.Picture.LastIndexOf("\\") + 1).Split('.')[0]),
                ResidenceID = p.T_PAPResidence.FirstOrDefault(pr => pr.IsPermanent).ResidenceId,
                ResidenceName = p.T_PAPResidence.FirstOrDefault(pr => pr.IsPermanent).T_Residence.ResidenceAddress,
                HouseHoldId = p.HouseHoldId
            });

            return pap.Union(pap2).OrderBy(pl => pl.LACId); ;
        }

        public PAPViewModel GetPAPDetails(int PapID)
        {
            var pap = db.T_PAP.Where(p => p.PAPId == PapID).ToList().Select(p => new PAPViewModel()
            {
                PAPId = p.PAPId,
                PersonId = p.T_Person.PersonId,
                FirstName = p.T_Person.FirstName,
                LastName = p.T_Person.LastName,
                MiddleName = p.T_Person.MiddleName,
                Mobile = p.T_Person.Mobile,
                Email = p.T_Person.Email,
                Gender = p.T_Person.Gender,
                DateOfBirth = p.T_Person.DateOfBirth,
                DateOfBirthString = p.T_Person.DateOfBirth?.ToString("dd/MMM/yyyy"),
                PlaceOfBirth = p.T_Person.PlaceOfBirth,
                Father = p.T_Person.Father,
                FatherName = p?.T_Person?.T_Person2?.FirstName + " " + p?.T_Person?.T_Person2?.LastName + "(" + p?.T_Person?.T_Person2?.PersonId + ")",
                Mother = p.T_Person.Mother,
                MotherName = p?.T_Person?.T_Person3?.FirstName + " " + p?.T_Person?.T_Person3?.LastName + "(" + p?.T_Person?.T_Person3?.PersonId + ")" ,
                Spouse = p.Spouse,
                SpouseName = p.T_Person1?.FirstName + " " + p.T_Person1?.LastName + "(" + p.T_Person1?.PersonId + ")",
                VulnerabilityTypeId = p.T_Person.VulnerabilityTypeId ?? default,
                VulnerabilityTypeName = (bool)p.T_Person.VulnerabilityTypeId.HasValue ? p.T_Person?.T_List?.ListValue : string.Empty,
                VulnerabilityDetail = p.T_Person.VulnerabilityDetail,
                IdCard = p.IdCard,
                Picture = PictureLink(p.PAPId, "PAP"),
                PhotoID = int.Parse(p.Picture.Substring(p.Picture.LastIndexOf("\\") + 1).Split('.')[0]),
                ResidenceID = p.T_PAPResidence.FirstOrDefault(pr => pr.IsPermanent).ResidenceId,
                ResidenceName = p.T_PAPResidence.FirstOrDefault(pr => pr.IsPermanent).T_Residence.ResidenceAddress,
                HouseHoldId = p.HouseHoldId
            }).FirstOrDefault();

            pap.FatherName = pap.FatherName == " ()" ? string.Empty : pap.FatherName;
            pap.MotherName = pap.MotherName == " ()" ? string.Empty : pap.MotherName;
            pap.SpouseName = pap.SpouseName == " ()" ? string.Empty : pap.SpouseName;
            return pap;
        }

        public PAPViewModel GetPAPDetails(int PersonID, int LacID)
        {
            int PapID = db.T_PAP.FirstOrDefault(p => p.PersonId == PersonID).PAPId;
            var pap = db.T_PAP.Where(p => p.PersonId == PersonID).ToList().Select(p => new PAPViewModel() 
            {
                PAPId = p.PAPId,
                PersonId = p.T_Person.PersonId,
                FirstName = p.T_Person.FirstName,
                LastName = p.T_Person.LastName,
                MiddleName = p.T_Person.MiddleName,
                Mobile = p.T_Person.Mobile,
                Email = p.T_Person.Email,
                Gender = p.T_Person.Gender,
                DateOfBirth = p.T_Person.DateOfBirth,
                DateOfBirthString = p.T_Person.DateOfBirth?.ToString("dd/MMM/yyyy"),
                PlaceOfBirth = p.T_Person.PlaceOfBirth,
                Father = p.T_Person.Father,
                FatherName = p.T_Person.Father.HasValue? p?.T_Person?.T_Person2?.FirstName + " " + p?.T_Person?.T_Person2?.LastName + "(" + p?.T_Person?.T_Person2?.PersonId + ")" : string.Empty,
                Mother = p.T_Person.Mother,
                MotherName = p.T_Person.Mother.HasValue ? p?.T_Person?.T_Person3?.FirstName + " " + p?.T_Person?.T_Person3?.LastName + "(" + p?.T_Person?.T_Person3?.PersonId + ")" : string.Empty,
                Spouse = p.Spouse,
                SpouseName = p.Spouse.HasValue? p.T_Person1?.FirstName + " " + p.T_Person1?.LastName + "(" + p.T_Person1?.PersonId + ")" : string.Empty,
                VulnerabilityTypeId = p.T_Person.VulnerabilityTypeId ?? default,
                VulnerabilityTypeName = (bool)p.T_Person.VulnerabilityTypeId.HasValue ? p.T_Person.T_List1.ListValue : "",
                VulnerabilityDetail = p.T_Person.VulnerabilityDetail,
                IdCard = p.IdCard,
                Picture = PictureLink(p.PAPId, "PAP"),
                PhotoID = int.Parse(p.Picture.Substring(p.Picture.LastIndexOf("\\") + 1).Split('.')[0]),
                ResidenceID = p.T_PAPResidence.FirstOrDefault(pr => pr.IsPermanent).ResidenceId,
                ResidenceName = p.T_PAPResidence.FirstOrDefault(pr => pr.IsPermanent).T_Residence.ResidenceAddress,
                FileNumber = p.T_PAPLAC?.FirstOrDefault(l=>l.LACId == LacID)?.FileNumber,
                HouseHoldId = p.HouseHoldId
            }).FirstOrDefault();

            pap.FatherName = !string.IsNullOrWhiteSpace(pap?.FatherName) ? pap.FatherName == " ()" ? string.Empty : pap.FatherName : string.Empty;
            pap.MotherName = !string.IsNullOrWhiteSpace(pap?.MotherName) ? pap.MotherName == " ()" ? string.Empty : pap.MotherName : string.Empty;
            pap.SpouseName = !string.IsNullOrWhiteSpace(pap?.SpouseName) ? pap.SpouseName == " ()" ? string.Empty : pap.SpouseName : string.Empty;
            return pap;
        }

        string PictureLink(int id, string type)
        {
            string[] pictureLink = null;
            if (type == "PAPLAC")
                pictureLink = db.T_PAPLAC.FirstOrDefault(pl => pl.PAPId == id).T_PAP.Picture.Split('\\');
            else if (type == "Property")
                pictureLink = db.T_Property.FirstOrDefault(pr => pr.PropertyId == id).Picture.Split('\\');
            else if (type == "PAP")
                pictureLink = db.T_PAP.FirstOrDefault(p => p.PAPId == id).Picture.Split('\\');

            string pic = "/Rap/";
            for (int i = 6; i < pictureLink.Length; i++)
            {
                pic += pictureLink[i] + "/";
            }
            pic = pic.Substring(0, pic.Length - 1);
            return pic;
        }

        public IEnumerable<PropertiesViewModel> LoadProperties(PAPViewModel pap)
        {
            var properties = db.T_Property.Where(p => (p.Owner == pap.PAPId || p.User == pap.PAPId)).ToList().
                Select(prop => new PropertiesViewModel() {
                    PropertyId = prop.PropertyId,
                    Owner = prop.Owner,
                    User = prop.User,
                    UserType = (prop.User == prop.Owner) ? "PU" : (prop.Owner == pap.PAPId ? "P" : "U"),
                    IsOwner = (prop.User == prop.Owner),
                    IsUser = (prop.Owner != pap.PAPId || prop.User == prop.Owner),
                    LACId = prop.LACId,
                    LACName = prop.T_LAC.LACName,
                    OwnerFirstName = prop.T_PAP.T_Person.FirstName,
                    OwnerLastName = prop.T_PAP.T_Person.LastName,
                    OwnerMiddleName = prop.T_PAP.T_Person.MiddleName,
                    OwnerFileNumber = prop.T_PAP.T_PAPLAC.FirstOrDefault().FileNumber,
                    IPointName = prop.T_Point1.PointName,
                    InvestigatorPoint = prop.InvestigatorPoint,
                    Investigator = prop.T_Employee.T_Person.PersonId,
                    InventoryDate = prop.InventoryDate,
                    InvestigatorCode = prop.T_Employee.EmployeeCode,
                    TopographerPoint = prop.TopographerPoint,
                    Picture = prop.PictureName.Split('.')[0],
                    PictureName = PictureLink(prop.PropertyId, "Property"),
                    PropertyType = prop.PropertyType,
                    PropertyTypeName = prop.T_AssetType.AssetName,
                    PropertyName = prop.PropertyName,
                    Comments = prop.Comments
                });
            return properties;
        }

        public IEnumerable<PropertiesViewModel> LoadProperties(PAPViewModel pap, int lacID)
        {
            var properties = db.T_Property.Where(p => (p.Owner == pap.PAPId || p.User == pap.PAPId) && p.LACId == lacID).ToList().
                Select(prop => new PropertiesViewModel()
                {
                    PropertyId = prop.PropertyId,
                    Owner = prop.Owner,
                    User = prop.User,
                    UserType = (prop.User == prop.Owner) ? "PU" : (prop.Owner == pap.PAPId ? "P" : "U"),
                    IsOwner = (prop.User == prop.Owner),
                    IsUser = (prop.Owner != pap.PAPId || prop.User == prop.Owner),
                    LACId = prop.LACId,
                    LACName = prop.T_LAC.LACName,
                    OwnerFirstName = prop.T_PAP?.T_Person?.FirstName,
                    OwnerLastName = prop.T_PAP?.T_Person?.LastName,
                    OwnerMiddleName = prop.T_PAP?.T_Person?.MiddleName,
                    OwnerFileNumber = prop.T_PAP?.T_PAPLAC?.FirstOrDefault()?.FileNumber,
                    IPointName = prop.T_Point1?.PointName,
                    InvestigatorPoint = prop.InvestigatorPoint,
                    Investigator = (int)prop.T_Employee?.T_Person?.PersonId,
                    InventoryDate = prop.InventoryDate,
                    InvestigatorCode = prop.T_Employee.EmployeeCode,
                    TopographerPoint = prop.TopographerPoint,
                    Picture = prop.PictureName.Split('.')[0],
                    PictureName = PictureLink(prop.PropertyId, "Property"),
                    PropertyType = prop.PropertyType,
                    PropertyTypeName = prop.T_AssetType.AssetName,
                    PropertyName = prop.PropertyName,
                    Comments = prop.Comments
                });
            return properties;
        }

        public IEnumerable<PropertiesViewModel> LoadProperties()
        {
            var properties = db.T_Property.ToList().
                Select(prop => new PropertiesViewModel()
                {
                    PropertyId = prop.PropertyId,
                    Owner = prop.Owner,
                    User = prop.User,
                    LACId = prop.LACId,
                    LACName = prop.T_LAC.LACName,
                    OwnerFirstName = prop.T_PAP.T_Person.FirstName,
                    OwnerLastName = prop.T_PAP.T_Person.LastName,
                    OwnerMiddleName = prop.T_PAP.T_Person.MiddleName,
                    OwnerFileNumber = prop.T_PAP.T_PAPLAC.FirstOrDefault().FileNumber,
                    IPointName = prop.T_Point1.PointName,
                    InvestigatorPoint = prop.InvestigatorPoint,
                    Investigator = prop.T_Employee.T_Person.PersonId,
                    InventoryDate = prop.InventoryDate,
                    InvestigatorCode = prop.T_Employee.EmployeeCode,
                    TopographerPoint = prop.TopographerPoint,
                    Picture = prop.PictureName.Split('.')[0],
                    PictureName = PictureLink(prop.PropertyId, "Property"),
                    PropertyType = prop.PropertyType,
                    PropertyTypeName = prop.T_AssetType.AssetName,
                    PropertyName = prop.PropertyName,
                    Comments = prop.Comments
                });
            return properties;
        }

        public PropertiesViewModel LoadProperties(int propertyID)
        {
            var properties = db.T_Property.Where(p => p.PropertyId == propertyID).ToList().
                Select(prop => new PropertiesViewModel()
                {
                    PropertyId = prop.PropertyId,
                    Owner = prop.Owner,
                    User = prop.User,
                    OwnerID = prop.Owner,
                    OwnerFirstName = prop.T_PAP.T_Person.FirstName,
                    OwnerLastName = prop.T_PAP.T_Person.LastName,
                    OwnerMiddleName = prop.T_PAP.T_Person.MiddleName,
                    OwnerFileNumber = prop.T_PAP.T_PAPLAC.FirstOrDefault().FileNumber,
                    OwnerPictureName = PictureLink(prop.T_PAP.PAPId, "PAPLAC"),
                    OwnerPrimaryResidenceId = prop.T_PAP.T_PAPResidence.FirstOrDefault().ResidenceId,
                    OwnerPrimaryResidenceName = prop.T_PAP.T_PAPResidence.FirstOrDefault().T_Residence.ResidenceAddress,
                    UserFirstName = prop.T_PAP1?.T_Person.FirstName,
                    UserLastName = prop.T_PAP1?.T_Person.LastName,
                    UserMiddleName = prop.T_PAP1?.T_Person.MiddleName,
                    UserFileNumber = (int)prop.T_PAP1?.T_PAPLAC.FirstOrDefault().FileNumber,
                    UserPictureName = PictureLink((int)prop.T_PAP1?.PAPId, "PAPLAC"),
                    UserPrimaryResidenceId = prop.T_PAP1?.T_PAPResidence?.FirstOrDefault()?.ResidenceId,
                    UserPrimaryResidenceName = prop.T_PAP1?.T_PAPResidence?.FirstOrDefault().T_Residence?.ResidenceAddress,
                    LandId = prop.LandId,
                    VillageId = prop.VillageId ?? 0,
                    VillageName = prop.T_Village?.VillageName,
                    LACId = prop.LACId,
                    LACName = prop.T_LAC.LACName,
                    Investigator = prop.Investigator,
                    InvestigatorCode = prop.T_Employee.EmployeeCode,
                    InvestigatorPoint = prop.InvestigatorPoint,
                    IPointName = prop.T_Point1.PointName,
                    IPointLatitude = prop.T_Point1.Latitude,
                    IPointLongitude = prop.T_Point1.Longitude,
                    IPointElevation = prop.T_Point1.Elevation,
                    TopographerPoint = prop.TopographerPoint,
                    TPointName = prop.T_Point2?.PointName,
                    TPointLatitude = prop.T_Point2?.Latitude,
                    TPointLongitude = prop.T_Point2?.Longitude,
                    TPointElevation = prop.T_Point2?.Elevation,
                    PictureName = PictureLink(prop.PropertyId, "Property"),
                    Picture = prop.PictureName.Split('.')[0],
                    PropertyType = prop.PropertyType,
                    PropertyTypeName = prop.T_AssetType.AssetName,
                    PropertyName = prop.PropertyName,
                    Surface = prop.Surface,
                    SurfaceUOM = prop.SurfaceUOM,
                    SurfaceUOMName = prop.T_UOM?.UOM,
                    SurfaceString = prop.Surface + " " + prop.T_UOM?.UOM,
                    CultureStartDate = prop.CultureStartDate,
                    CultureOccupationType = prop.CultureOccupationType,
                    CultureOccupationName = prop.T_List?.ListValue,
                    CultureStartDateString = prop.CultureStartDate?.ToString("dd/MMMM/yyyy"),
                    TreeMaturity = prop.TreeMaturity,
                    TreeMaturityName = prop.T_List5?.ListValue,
                    TreeQty = prop.TreeQty,
                    StructureType = prop.StructureType,
                    StructureTypeName = prop.T_List3?.ListValue,
                    RoomQty = prop.RoomQty,
                    StructureLength = prop.StructureLength,
                    StructureWidth = prop.StructureWidth,
                    RoofType = prop.RoofType,
                    RoofTypeName = prop.T_List1?.ListValue,
                    WallType = prop.WallType,
                    WallTypeName = prop.T_List6?.ListValue,
                    SoilType = prop.SoilType,
                    SoilTypeName = prop.T_List2?.ListValue,
                    StructureUOM = prop.StructureUOM,
                    StructureUOMString = prop.T_UOM1?.UOM,
                    StructureUsageID = prop.StructureUsage,
                    StructureUsageName = prop.T_List4?.ListValue,
                    MeasurementSignatureDate = prop.MeasurementSignatureDate,
                    MeasurementSignatureDateString = prop.MeasurementSignatureDate?.ToString("dd/MMMM/yyyy"),
                    TopographerId = prop.TopographerId,
                    TopographerCode = prop.T_Employee1?.EmployeeCode,
                    MeasurementDate = prop.MeasurementDate,
                    MeasurementDateString = prop.MeasurementDate?.ToString("dd/MMMM/yyyy"),
                    GPSDevice = prop.GPSDevice,
                    InventoryDate = prop.InventoryDate,
                    InventoryDateString = prop.InventoryDate.ToString("dd/MMMM/yyyy"),
                    Comments = prop.Comments,
                    IsHabitable = prop.IsHabitable,
                    HasOwner = prop.HasOwner
                }).FirstOrDefault();
            return properties;
        }

        #endregion

        #region CRUD LAND MANAGEMENT
        public List<string> AddPAPLAC(PAPViewModel pap, HttpPostedFileBase[] attachments)
        {
            List<string> error = new List<string> { "Error" };
            List<string> success = new List<string> { "Success","PAP added!" };

            if (Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-POWER") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-LO") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-INV"))
            {
                if (pap == null)
                    error.Add("Can't save an empty PAP");
                if (pap.Properties.Length < 1)
                    error.Add("Can't add a PAP without properties");
                if (pap.PersonId < 1)
                {
                    if (string.IsNullOrWhiteSpace(pap.FirstName))
                        error.Add("PAP first Name is empty");
                    if (string.IsNullOrWhiteSpace(pap.LastName))
                        error.Add("PAP Last Name is empty");
                    if (string.IsNullOrWhiteSpace(pap.ResidenceName))
                        error.Add("You didn't define a valid primary residence for this PAP");
                    if (pap.PhotoID < 1)
                        error.Add("Photo ID can't be empty");
                    if (string.IsNullOrWhiteSpace(pap.Picture))
                        error.Add("You didn't upload the PAP picture");
                }

                if (string.IsNullOrWhiteSpace(pap.PresurveyorCode))
                    error.Add("Investigator Code can't be empty");
                if (string.IsNullOrWhiteSpace(pap.PresurveyorGPS))
                    error.Add("Investigator GPS tools can't be empty");
                if (pap.FileNumber < 1)
                    error.Add("PAP File number is invalide");
                if (pap.LACId < 1)
                    error.Add("LAC Id is not valid. Please refresh the page and try again");
                if (pap.PresurveyDate > DateTime.Now)
                    error.Add("Pre-Survey date can't bee in the future");

                foreach (var property in pap.Properties)
                {
                    if (string.IsNullOrWhiteSpace(property.IPointName))
                        error.Add("Can not add a property without GPS ID");
                    if (property.IPointLongitude.Equals(null))
                        error.Add("Easting value can't be empty");
                    if (property.IPointLatitude.Equals(null))
                        error.Add("Northing value can't be empty");
                    if (property.PropertyType < 1)
                        error.Add("You did not provide property");
                    if (string.IsNullOrWhiteSpace(property.Picture))
                        error.Add("Please provide Picture ID");
                    if (string.IsNullOrWhiteSpace(property.PictureName))
                        error.Add("Please upload property Picture");
                    if (string.IsNullOrWhiteSpace(property.PropertyName))
                        error.Add("Property Name can not be empty");
                    if (property.PropertyTypeName == "Tree")
                    {
                        if (property.TreeMaturity < 1)
                            error.Add("You did not provide tree maturity");
                        if (property.TreeQty < 1)
                            error.Add("Please specify tree quantity");
                    }
                    if (property.PropertyTypeName == "Culture")
                    {
                        if (property.CultureOccupationType < 1)
                            error.Add("You did not specify culture Type");
                        if (property.Surface < 1)
                            error.Add("Culture Surface can not be less than 1");
                    }
                    if (property.PropertyTypeName == "Structure")
                    {
                        if (property.StructureType < 1)
                            error.Add("Please specify Structure Type");
                        if (property.RoofType < 1)
                            error.Add("Please specify Roof Type");
                        if (property.WallType < 1)
                            error.Add("Please specify Wall Type");
                        if (property.SoilType < 1)
                            error.Add("Please specify Floor Type");
                        if (property.StructureLength < 1)
                            error.Add("Structure length can not be less than 1");
                        if (property.StructureWidth < 1)
                            error.Add("Structure width can not be less than 1");
                        if (property.RoomQty < 1)
                            error.Add("Room quantity can not be less than 1");
                        if (property.StructureUsageID < 1)
                            error.Add("Specify the valid Structure usage");
                    }
                    if (property.UserType == "U")
                    {
                        if (!property.OwnerID.HasValue)
                        {
                            if (string.IsNullOrWhiteSpace(property.OwnerFirstName))
                                error.Add("Please add the Owner's Last Name");
                            if (string.IsNullOrWhiteSpace(property.OwnerLastName))
                                error.Add("Please add the Owner's Last Name");
                            if (string.IsNullOrWhiteSpace(property.OwnerPrimaryResidenceName) || property.OwnerPrimaryResidenceId < 1)
                                error.Add("You did not choose the primary residence of the owner");
                        }
                        if (property.OwnerFileNumber < 1)
                            error.Add("Please specify a valid File Number for the owner");
                        if (property.OwnerFileNumber == pap.FileNumber)
                            error.Add("Owner file number can not be the same as User's File Number");
                    }
                }

                if (error.Count > 1)
                    return error;
                else
                {
                    using (var dbTransaction = db.Database.BeginTransaction())
                    {
                        int ownerPapId = pap.PAPId;
                        try
                        {
                            //Save the PAP in Person Table after checking if the person already exists
                            var person = pap.PersonId > 0 ? db.T_Person.Find(pap.PersonId) : db.T_Person.FirstOrDefault(p => p.FirstName == pap.FirstName && p.LastName == pap.LastName && p.MiddleName == pap.MiddleName);
                            if (person == null)
                            {
                                person = new T_Person()
                                {
                                    PersonType = "PAP",
                                    FirstName = pap.FirstName,
                                    LastName = pap.LastName,
                                    MiddleName = pap.MiddleName,
                                    Created = DateTime.Now,
                                    CreatedBy = loggedUser,
                                    Updated = DateTime.Now,
                                    UpdatedBy = loggedUser
                                };
                                db.T_Person.Add(person);
                                db.SaveChanges();
                            }
                            pap.PersonId = person.PersonId;

                            //Save PAP in PAP Table after checking if the pap already exists
                            var existingPAP = db.T_PAP.Where(p => p.PersonId == pap.PersonId).FirstOrDefault();
                            if (existingPAP == null)
                            {
                                existingPAP = new T_PAP()
                                {
                                    PersonId = person.PersonId,
                                    Picture = pap.Picture,
                                    Created = DateTime.Now,
                                    CreatedBy = loggedUser,
                                    Updated = DateTime.Now,
                                    UpdatedBy = loggedUser
                                };
                                db.T_PAP.Add(existingPAP);
                                db.SaveChanges();
                            }
                            pap.PAPId = existingPAP.PAPId;
                            ownerPapId = pap.PAPId;

                            //Save PAP Attachment in T_Attachement and then in PAPAttachment
                            if (!string.IsNullOrWhiteSpace(pap.Picture)) 
                            {
                                string pictureName = pap.Picture?.Split('\\')[2];
                                string papServerPath = folderRoot + @"PAP\" + pap.PAPId.ToString() + @"\";
                                foreach (var file in attachments)
                                {
                                    if (file.FileName == pictureName)
                                    {
                                        if (!Directory.Exists(papServerPath))
                                            Directory.CreateDirectory(papServerPath);

                                        papServerPath = Path.Combine(papServerPath, Path.GetFileName(file.FileName));
                                        file.SaveAs(papServerPath);
                                        var currentPap = db.T_PAP.Find(pap.PAPId);
                                        currentPap.Picture = papServerPath;
                                        db.Entry(currentPap).State = EntityState.Modified;
                                        break;
                                    }
                                }
                            }
                            
                            var residence = db.T_Residence.FirstOrDefault(r => r.ResidenceAddress == pap.ResidenceName);
                            if (residence == null)
                            {
                                residence = new T_Residence()
                                {
                                    ResidenceAddress = pap.ResidenceName,
                                    Created = DateTime.Now,
                                    CreatedBy = loggedUser,
                                    Updated = DateTime.Now,
                                    UpdatedBy = loggedUser
                                };
                                db.T_Residence.Add(residence);
                                db.SaveChanges();
                            }

                            var papRes = db.T_PAPResidence.FirstOrDefault(r=>r.ResidenceId == residence.ResidenceId && r.PAPId == pap.PAPId);
                            if(papRes == null)
                            {
                                db.T_PAPResidence.Add(new T_PAPResidence()
                                {
                                    PAPId = pap.PAPId,
                                    ResidenceId = residence.ResidenceId,
                                    IsPermanent = true,
                                    Created = DateTime.Now,
                                    CreatedBy = loggedUser,
                                    Updated = DateTime.Now,
                                    UpdateBy = loggedUser
                                });
                                db.SaveChanges();
                            }

                            //Save PAPLAC
                            var personlac = db.T_PAPLAC.FirstOrDefault(p => p.LACId == pap.LACId && p.PAPId == pap.PAPId);
                            if (personlac == null)
                            {
                                personlac = new T_PAPLAC()
                                {
                                    LACId = pap.LACId,
                                    PAPId = pap.PAPId,
                                    PresurveyDate = pap.PresurveyDate,
                                    Presurveyor = db.T_Employee.FirstOrDefault(e => e.EmployeeCode == pap.PresurveyorCode).EmployeeId,
                                    PresurveyCamera = pap.PresurveyorCamera,
                                    PresurveyGPS = pap.PresurveyorGPS,
                                    FileNumber = pap.FileNumber.Value,
                                    Created = DateTime.Now,
                                    CreatedBy = loggedUser,
                                    Updated = DateTime.Now,
                                    UpdatedBy = loggedUser
                                };
                                db.T_PAPLAC.Add(personlac);
                                db.SaveChanges();
                            }
                            else
                            {
                                personlac.PresurveyGPS = pap.PresurveyorGPS;
                                personlac.PresurveyCamera = pap.PresurveyorCamera;
                                personlac.Updated = DateTime.Now;
                                personlac.UpdatedBy = loggedUser;

                                db.Entry(personlac).State = EntityState.Modified;
                                db.SaveChanges();
                            }

                            foreach (var prop in pap.Properties)
                            {
                                var lacRequestId = db.T_LAC.FirstOrDefault(l => l.LACId == pap.LACId).LACRequestId;
                                var landId = db.T_Land.FirstOrDefault(l => l.LACRequestId == lacRequestId).LandId;
                                T_Point point = db.T_Point.Where(p => p.LandId == landId && p.Latitude == prop.IPointLatitude && p.Longitude == prop.IPointLongitude && p.PointName == prop.IPointName).FirstOrDefault();
                                if (point == null)
                                {
                                    point = new T_Point()
                                    {
                                        PointName = prop.IPointName,
                                        Elevation = prop.IPointElevation,
                                        Longitude = prop.IPointLongitude,
                                        Latitude = prop.IPointLatitude,
                                        LandId = landId,
                                        Created = DateTime.Now,
                                        CreatedBy = loggedUser,
                                        Updated = DateTime.Now,
                                        UpdatedBy = loggedUser
                                    };
                                    db.T_Point.Add(point);
                                    db.SaveChanges();
                                }

                                if (prop.UserType == "U" || prop.UserType == "P")
                                {
                                    //Insert Owner in T_Person, T_PAP, T_PAPLAC, T_PAPAttachment, T_PAPLACAttachment
                                    T_Person owner = prop.OwnerID.HasValue ? db.T_Person.Find(prop.OwnerID.Value) : db.T_Person.Where(p => p.FirstName == prop.OwnerFirstName && p.LastName == prop.OwnerLastName && p.MiddleName == prop.OwnerMiddleName).FirstOrDefault();
                                    if (owner == null)
                                    {
                                        owner = new T_Person()
                                        {
                                            PersonType = "PAP",
                                            FirstName = prop.OwnerFirstName,
                                            LastName = prop.OwnerLastName,
                                            MiddleName = prop.OwnerMiddleName,
                                            Created = DateTime.Now,
                                            CreatedBy = loggedUser,
                                            Updated = DateTime.Now,
                                            UpdatedBy = loggedUser
                                        };
                                        db.T_Person.Add(owner);
                                        db.SaveChanges();
                                    }

                                    var ownerPAP = db.T_PAP.Where(p => p.PersonId == owner.PersonId).FirstOrDefault();
                                    if (ownerPAP == null)
                                    {
                                        ownerPAP = new T_PAP()
                                        {
                                            PersonId = owner.PersonId,
                                            Picture = prop.OwnerPictureName,
                                            Created = DateTime.Now,
                                            CreatedBy = loggedUser,
                                            Updated = DateTime.Now,
                                            UpdatedBy = loggedUser
                                        };
                                        db.T_PAP.Add(ownerPAP);
                                        db.SaveChanges();

                                        //Save PAP Attachment in T_Attachement and then in PAPAttachment
                                        string ownerPictureName = ownerPAP.Picture.Split('\\')[2];
                                        string ownerServerPath = folderRoot + @"PAP\" + ownerPapId.ToString() + @"\";
                                        foreach (var file in attachments)
                                        {
                                            if (file.FileName == ownerPictureName)
                                            {
                                                if (!Directory.Exists(ownerServerPath))
                                                    Directory.CreateDirectory(ownerServerPath);

                                                ownerServerPath = Path.Combine(ownerServerPath, Path.GetFileName(file.FileName));
                                                file.SaveAs(ownerServerPath);
                                                var ownerPap = db.T_PAP.Find(ownerPapId);
                                                ownerPap.Picture = ownerServerPath;
                                                db.Entry(ownerPap).State = EntityState.Modified;
                                                break;
                                            }
                                        }

                                        var ownerRes = db.T_Residence.FirstOrDefault(r => r.ResidenceAddress == prop.OwnerPrimaryResidenceName);
                                        if (ownerRes == null)
                                        {
                                            ownerRes = new T_Residence()
                                            {
                                                ResidenceAddress = pap.ResidenceName,
                                                Created = DateTime.Now,
                                                CreatedBy = loggedUser,
                                                Updated = DateTime.Now,
                                                UpdatedBy = loggedUser
                                            };
                                            db.T_Residence.Add(ownerRes);
                                            db.SaveChanges();
                                        }

                                        db.T_PAPResidence.Add(new T_PAPResidence()
                                        {
                                            PAPId = ownerPapId,
                                            ResidenceId = ownerRes.ResidenceId,
                                            IsPermanent = true,
                                            Created = DateTime.Now,
                                            CreatedBy = loggedUser,
                                            UpdateBy = loggedUser,
                                            Updated = DateTime.Now
                                        });
                                        db.SaveChanges();
                                    }

                                    ownerPapId = ownerPAP.PAPId;
                                    var paplac = db.T_PAPLAC.FirstOrDefault(p => p.PAPId == ownerPapId && p.LACId == pap.LACId);
                                    if (paplac == null)
                                    {
                                        db.T_PAPLAC.Add(new T_PAPLAC()
                                        {
                                            LACId = pap.LACId,
                                            PAPId = ownerPapId,
                                            PresurveyDate = pap.PresurveyDate,
                                            Presurveyor = db.T_Employee.FirstOrDefault(e => e.EmployeeCode == pap.PresurveyorCode).EmployeeId,
                                            PresurveyGPS = pap.PresurveyorGPS,
                                            PresurveyCamera = pap.PresurveyorCamera,
                                            FileNumber = prop.OwnerFileNumber.Value,
                                            Created = DateTime.Now,
                                            CreatedBy = loggedUser,
                                            Updated = DateTime.Now,
                                            UpdatedBy = loggedUser
                                        });
                                        db.SaveChanges();
                                    }
                                    else
                                    {
                                        paplac.PresurveyCamera = pap.PresurveyorCamera;
                                        paplac.PresurveyDate = pap.PresurveyDate;
                                        paplac.PresurveyGPS = pap.PresurveyorGPS;
                                        paplac.Updated = DateTime.Now;
                                        paplac.UpdatedBy = loggedUser;
                                        db.Entry(paplac).State = EntityState.Modified;
                                    }
                                }

                                //Save property picture before property
                                string proPictureName = prop.PictureName.Split('\\')[2];
                                string proServerPath = folderRoot + @"PAP\" + (prop.UserType == "U" ? ownerPapId.ToString() : pap.PAPId.ToString()) + @"\Properties\LAC" + pap.LACId.ToString() + @"\";
                                foreach (var file in attachments)
                                {
                                    if (file.FileName == proPictureName)
                                    {
                                        if (!Directory.Exists(proServerPath))
                                            Directory.CreateDirectory(proServerPath);

                                        proServerPath = Path.Combine(proServerPath, Path.GetFileName(file.FileName));
                                        file.SaveAs(proServerPath);
                                        break;
                                    }
                                }

                                //Insert Property
                                db.T_Property.Add(new T_Property()
                                {
                                    Owner = (prop.UserType == "P" || prop.UserType == "PU") ? pap.PAPId : ownerPapId,
                                    User = prop.UserType == "U" ? pap.PAPId : ownerPapId,
                                    LandId = landId,
                                    LACId = pap.LACId,
                                    Investigator = db.T_Employee.FirstOrDefault(e => e.EmployeeCode == pap.PresurveyorCode).EmployeeId,
                                    InvestigatorPoint = point.PointId,
                                    Picture = proServerPath,
                                    PictureName = proPictureName,
                                    PropertyType = prop.PropertyType,
                                    PropertyName = prop.PropertyName,
                                    Surface = prop.Surface,
                                    SurfaceUOM = prop.SurfaceUOM,
                                    CultureOccupationType = prop.CultureOccupationType,
                                    TreeMaturity = prop.TreeMaturity,
                                    TreeQty = prop.TreeQty,
                                    StructureType = prop.StructureType,
                                    StructureLength = prop.StructureLength,
                                    StructureWidth = prop.StructureWidth,
                                    RoomQty = prop.RoomQty,
                                    RoofType = prop.RoofType,
                                    WallType = prop.WallType,
                                    SoilType = prop.SoilType,
                                    StructureUOM = prop.StructureUOM,
                                    StructureUsage = prop.StructureUsageID,
                                    GPSDevice = pap.PresurveyorGPS,
                                    InventoryDate = (DateTime)pap.PresurveyDate,
                                    CameraCode = pap.PresurveyorCamera,
                                    Comments = prop.Comments,
                                    Created = DateTime.Now,
                                    CreatedBy = loggedUser,
                                    Updated = DateTime.Now,
                                    UpdatedBy = loggedUser
                                });
                                db.SaveChanges();
                            }

                            dbTransaction.Commit();
                            return success;
                        }
                        catch (Exception e)
                        {
                            try
                            {
                                dbTransaction.Rollback();
                                string papServerPath = folderRoot + @"PAP\" + pap.PAPId.ToString() + @"\";
                                if (Directory.Exists(papServerPath))
                                    Directory.Delete(papServerPath, true);

                                string ownerServerPath = folderRoot + @"PAP\" + ownerPapId.ToString() + @"\";
                                if (Directory.Exists(ownerServerPath))
                                    Directory.Delete(ownerServerPath, true);
                            }
                            catch (Exception)
                            {
                            }
                            List<string> exception = new List<string> { "Exception", e.Message };
                            return exception;
                        }
                    }
                }
            }
            else
            {
                error.Add("You don't have sufficient permissions to perform this task.");
                return error;
            }
        }

        public List<string> AddPAPLAC(PAPViewModel pap)
        {
            List<string> error = new List<string> { "Error" };
            List<string> success = new List<string> { "Success"};

            if (Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-POWER") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-LO") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-INV") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-TOPO"))
            {
                if (pap.PresurveyDate > DateTime.Now)
                    error.Add("Pre-Survey date can't be in the future");
                if (pap.SurveyDate > DateTime.Now)
                    error.Add("Survey date can't be in the future");
                if (pap.FormSubmissionDate > DateTime.Now)
                    error.Add("Form submission date can't be in the future");
                if (pap.SurveyDate < pap.PresurveyDate)
                    error.Add("Survey Date can't be less than Presurvey");
                if (pap.LACId < 1)
                    error.Add("Invalid Lac, please select a valid LAC");
                if (string.IsNullOrWhiteSpace(pap.PAPId.ToString()))
                    error.Add("Please refresh the page and select PAP again.");
                if (pap.FileNumber < 1 || string.IsNullOrWhiteSpace(pap.FileNumber.ToString()))
                    error.Add("You did not provide correct File Number");
                if (string.IsNullOrWhiteSpace(pap.PresurveyorCode))
                    error.Add("You need to determine the pre-surveyor");
                if (!DateTime.TryParse(pap.PresurveyDate.ToString(), out DateTime presurveyDate))
                    error.Add("Presurvey Date is not a valid date. Please choose a correct one");

                if (error.Count > 1)
                    return error;
                else
                {
                    //Insert into table T_PAPLAC
                    try
                    {
                        //checking if the pap already linked to lac
                        var paplac = db.T_PAPLAC.FirstOrDefault(p => p.PAPId == pap.PAPId && p.LACId == pap.LACId);
                        if (paplac == null)
                        {
                            pap.Presurveyor = db.T_Employee.FirstOrDefault(e => e.EmployeeCode == pap.PresurveyorCode).EmployeeId;
                            pap.Surveyor = !string.IsNullOrWhiteSpace(pap.SurveyorCode) ? db.T_Employee.FirstOrDefault(e => e.EmployeeCode == pap.SurveyorCode).EmployeeId : default(int?);

                            db.T_PAPLAC.Add(new T_PAPLAC()
                            {
                                LACId = pap.LACId,
                                PAPId = pap.PAPId,
                                PaymentPreference = pap.PaymentPreference == -1 ? default : pap.PaymentPreference,
                                FormSubmissionDate = pap.FormSubmissionDate == DateTime.Today ? default(DateTime?) : pap.FormSubmissionDate,
                                PresurveyDate = pap.PresurveyDate,
                                Presurveyor = pap.Presurveyor,
                                SurveyDate = pap.SurveyDate == DateTime.Today ? default : pap.SurveyDate,
                                Surveyor = pap.Surveyor,
                                FileNumber = pap.FileNumber.Value,
                                Comments = pap.Comments,
                                Created = DateTime.Now,
                                CreatedBy = loggedUser,
                                Updated = DateTime.Now,
                                UpdatedBy = loggedUser
                            });
                            db.SaveChanges();
                            success.Add("The PAP " + pap.PAPId + " has been linked to LAC " + pap.LACName + " successfully");
                            return success;
                        }
                        else
                        {
                            error.Add("This PAP " + pap.PAPId + " is already attached to this LAC " + pap.LACName);
                            return error;
                        }
                    }
                    catch (Exception e)
                    {
                        List<string> exception = new List<string> { "Exception", e.Message };
                        return exception;
                    }
                }
            }
            else
            {
                error.Add("You don't have sufficient permissions to perform this task.");
                return error;
            }
        }

        public List<string> AddPAP(PAPViewModel pap, bool addPapToLac, bool isUpdate)
        {
            List<string> error = new List<string> { "Error" };
            List<string> success = new List<string> { "Success"};

            if (Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-POWER") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-LO") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-INV") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-TOPO") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-POWER"))
            {
                if (string.IsNullOrWhiteSpace(pap.FirstName))
                    error.Add("Pap First Name can't be empty");
                if (string.IsNullOrWhiteSpace(pap.LastName))
                    error.Add("Pap Last Name can't be empty");
                if (pap.DateOfBirth >= DateTime.Today)
                    error.Add("PAP DOB can't be today");
                if (string.IsNullOrWhiteSpace(pap.ResidenceName))
                    error.Add("You didn't define a valid primary residence for this PAP");
                if (pap.PhotoID < 1)
                    error.Add("Photo ID can't be empty");
                if (string.IsNullOrWhiteSpace(pap.Picture) && !isUpdate)
                    error.Add("You didn't upload the PAP picture");
                if (pap.PAPFile.Count() < 1 && !isUpdate)
                    error.Add("You can't add a PAP without picture");

                if (addPapToLac)
                {
                    if (pap.LACId < 1)
                        error.Add("Invalid LAC ID. Please refresh the page and try again later.");
                    if (pap.LACName == "Select LAC")
                        error.Add("Please select a valid LAC");
                    if (pap.PresurveyDate > DateTime.Now)
                        error.Add("Pre-Survey date can't bee in the future");
                    if (string.IsNullOrWhiteSpace(pap.PresurveyorCode))
                        error.Add("Investigator Code can't be empty");
                    if (pap.FileNumber < 1 || string.IsNullOrWhiteSpace(pap.FileNumber.ToString()))
                        error.Add("PAP File number is invalide");
                }

                if (error.Count > 1)
                    return error;
                else
                {
                    using (var dbTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            string spouseId = !string.IsNullOrWhiteSpace(pap.SpouseName) ? (pap.SpouseName.Contains("(") ? (pap.SpouseName?.Split('(')[1])?.Substring(0, (int)(pap.SpouseName?.Split('(')[1])?.Length - 1) : null) : null;
                            string motherId = !string.IsNullOrWhiteSpace(pap.MotherName) ? (pap.MotherName.Contains("(") ? (pap.MotherName?.Split('(')[1])?.Substring(0, (int)(pap.MotherName?.Split('(')[1])?.Length - 1) : null) : null;
                            string fatherId = !string.IsNullOrWhiteSpace(pap.FatherName) ? (pap.FatherName.Contains("(") ? (pap.FatherName?.Split('(')[1])?.Substring(0, (int)(pap.FatherName?.Split('(')[1])?.Length - 1) : null) : null;
                            int? spouse = !string.IsNullOrWhiteSpace(spouseId) ? int.Parse(spouseId) : default(int?);
                            int? mother = !string.IsNullOrWhiteSpace(motherId) ? int.Parse(motherId) : default(int?);
                            int? father = !string.IsNullOrWhiteSpace(fatherId) ? int.Parse(fatherId) : default(int?);

                            var person = !isUpdate ? db.T_Person.FirstOrDefault(p => p.FirstName == pap.FirstName && p.LastName == pap.LastName && p.MiddleName == pap.MiddleName) : db.T_Person.FirstOrDefault(p => p.PersonId == pap.PersonId);

                            int defaultVulnerability = db.T_List.FirstOrDefault(l => l.ListName == "Vulnerability" && l.ListValue == "None").ListId;
                            pap.VulnerabilityTypeId = pap.VulnerabilityTypeId == -1 ? defaultVulnerability : pap.VulnerabilityTypeId;

                            //Check if parents has value before inserting the current pap
                            if(!mother.HasValue && !string.IsNullOrWhiteSpace(pap.MotherName))
                            {
                                var motherNames = pap.MotherName?.Split(' ');
                                if (motherNames.Length > 0)
                                {
                                    var _mother = db.T_Person.FirstOrDefault();
                                    if (motherNames.Length > 2)
                                        _mother = db.T_Person.FirstOrDefault(p => (p.FirstName == motherNames[0] && p.MiddleName == motherNames[1] && p.LastName == motherNames[2]) || (p.FirstName == motherNames[0] && p.MiddleName == motherNames[1] && p.LastName == motherNames[2]));
                                    else
                                        _mother = db.T_Person.FirstOrDefault(p => p.FirstName == motherNames[0] || p.MiddleName == motherNames[1] || p.LastName == motherNames[2]);

                                    if (_mother == null)
                                    {
                                        _mother = new T_Person()
                                        {

                                        };
                                        db.T_Person.Add(_mother);
                                    }
                                    mother = _mother.PersonId;
                                }
                            }

                            if (person == null)
                            {
                                person = new T_Person()
                                {
                                    PersonType = "PAP",
                                    FirstName = pap.FirstName,
                                    LastName = pap.LastName,
                                    MiddleName = pap.MiddleName,
                                    Mobile = pap.Mobile,
                                    Email = pap.Email,
                                    Gender = pap.Gender,
                                    IdCard = pap.IdCard,
                                    DateOfBirth = pap.DateOfBirth,
                                    PlaceOfBirth = pap.PlaceOfBirth,
                                    Father = father,
                                    Mother = mother,
                                    VulnerabilityTypeId = pap.VulnerabilityTypeId,
                                    VulnerabilityDetail = pap.VulnerabilityDetail,
                                    Created = DateTime.Now,
                                    CreatedBy = loggedUser,
                                    Updated = DateTime.Now,
                                    UpdatedBy = loggedUser
                                };
                                db.T_Person.Add(person);
                            }
                            else
                            {
                                person.FirstName = pap.FirstName;
                                person.LastName = pap.LastName;
                                person.MiddleName = pap.MiddleName;
                                person.Mobile = pap.Mobile;
                                person.Email = pap.Email;
                                person.Gender = pap.Gender;
                                person.IdCard = pap.IdCard;
                                person.DateOfBirth = pap.DateOfBirth;
                                person.PlaceOfBirth = pap.PlaceOfBirth;
                                person.Father = father;
                                person.Mother = mother;
                                person.VulnerabilityTypeId = pap.VulnerabilityTypeId;
                                person.VulnerabilityDetail = pap.VulnerabilityDetail;
                                person.Updated = DateTime.Now;
                                person.UpdatedBy = loggedUser;
                                db.Entry(person).State = EntityState.Modified;
                            }
                            db.SaveChanges();
                            pap.PersonId = person.PersonId;

                            //Save PAP in PAP Table after checking if the pap already exists
                            var existingPAP = isUpdate ? db.T_PAP.Where(p => p.PersonId == pap.PersonId).FirstOrDefault() : db.T_PAP.Find(pap.PAPId);
                            if (existingPAP == null)
                            {
                                existingPAP = new T_PAP()
                                {
                                    PersonId = person.PersonId,
                                    Picture = pap.Picture,
                                    Spouse = spouse,
                                    IdCard = pap.IdCard,
                                    Created = DateTime.Now,
                                    CreatedBy = loggedUser,
                                    Updated = DateTime.Now,
                                    UpdatedBy = loggedUser
                                };
                                db.T_PAP.Add(existingPAP);
                            }
                            else
                            {
                                existingPAP.Spouse = spouse;
                                existingPAP.IdCard = pap.IdCard;
                                existingPAP.Updated = DateTime.Now;
                                existingPAP.UpdatedBy = loggedUser;
                                db.Entry(existingPAP).State = EntityState.Modified;
                            }
                            db.SaveChanges();
                            pap.PAPId = existingPAP.PAPId;

                            //Save PAP Picture
                            if (pap.PAPFile.Length > 0)
                            {
                                string pictureName = pap.Picture.Split('\\')[2];
                                string papServerPath = @"\\fgmfp2\data\RAP\RAPSys\PAP\" + pap.PAPId.ToString() + @"\";
                                foreach (var file in pap.PAPFile)
                                {
                                    if (file.FileName == pictureName)
                                    {
                                        if (!Directory.Exists(papServerPath))
                                            Directory.CreateDirectory(papServerPath);

                                        if (File.Exists(existingPAP.Picture))
                                            File.Delete(existingPAP.Picture);

                                        papServerPath = Path.Combine(papServerPath, Path.GetFileName(file.FileName));
                                        file.SaveAs(papServerPath);
                                        var currentPap = db.T_PAP.Find(pap.PAPId);
                                        currentPap.Picture = papServerPath;
                                        db.Entry(currentPap).State = EntityState.Modified;
                                        break;
                                    }
                                }
                            }

                            var residence = db.T_Residence.FirstOrDefault(r => r.ResidenceAddress == pap.ResidenceName);
                            if (residence == null)
                            {
                                residence = new T_Residence()
                                {
                                    ResidenceAddress = pap.ResidenceName,
                                    Created = DateTime.Now,
                                    CreatedBy = loggedUser,
                                    Updated = DateTime.Now,
                                    UpdatedBy = loggedUser
                                };
                                db.T_Residence.Add(residence);
                            }
                            else
                            {
                                residence.ResidenceAddress = pap.ResidenceName;
                                residence.Updated = DateTime.Now;
                                residence.UpdatedBy = loggedUser;
                                db.Entry(residence).State = EntityState.Modified;
                            }
                            db.SaveChanges();

                            var papRes = db.T_PAPResidence.FirstOrDefault(r => r.PAPId == pap.PAPId && r.ResidenceId == residence.ResidenceId);
                            if (papRes == null)
                            {
                                db.T_PAPResidence.Add(new T_PAPResidence()
                                {
                                    PAPId = pap.PAPId,
                                    ResidenceId = residence.ResidenceId,
                                    IsPermanent = true,
                                    Created = DateTime.Now,
                                    CreatedBy = loggedUser,
                                    Updated = DateTime.Now,
                                    UpdateBy = loggedUser
                                });
                            }
                            else
                            {
                                papRes.IsPermanent = true;
                                papRes.Updated = DateTime.Now;
                                papRes.UpdateBy = loggedUser;
                                db.Entry(papRes).State = EntityState.Modified;

                                var residences = db.T_PAPResidence.Where(r => r.PAPId == pap.PAPId && r.IsPermanent && r.ResidenceId != residence.ResidenceId);
                                residences?.ToList().ForEach(r => r.IsPermanent = false);
                            }

                            db.SaveChanges();
                            success.Add("PAP " + pap.PAPId + (isUpdate ? " updated " : " created ") + " successfully!");

                            if (addPapToLac)
                            {
                                pap.Presurveyor = db.T_Employee.FirstOrDefault(e => e.EmployeeCode == pap.PresurveyorCode).EmployeeId;
                                pap.Surveyor = (pap.SurveyorCode == null) ? db.T_Employee.FirstOrDefault(e => e.EmployeeCode == pap.SurveyorCode).EmployeeId : default(int?);
                                //Save PAPLAC
                                T_PAPLAC papLac = new T_PAPLAC()
                                {
                                    LACId = pap.LACId,
                                    PAPId = pap.PAPId,
                                    PaymentPreference = pap.PaymentPreference == -1 ? default : pap.PaymentPreference,
                                    FormSubmissionDate = pap.FormSubmissionDate == DateTime.Today ? default : pap.FormSubmissionDate,
                                    PresurveyDate = pap.PresurveyDate,
                                    Presurveyor = pap.Presurveyor,
                                    SurveyDate = pap.SurveyDate == DateTime.Today ? default : pap.SurveyDate,
                                    Surveyor = pap.Surveyor,
                                    FileNumber = pap.FileNumber.Value,
                                    Comments = pap.Comments,
                                    Created = DateTime.Now,
                                    CreatedBy = loggedUser,
                                    Updated = DateTime.Now,
                                    UpdatedBy = loggedUser
                                };
                                db.T_PAPLAC.Add(papLac);
                                db.SaveChanges();
                                success.Add("PAP " + pap.PAPId + " created successfully, and added to LAC " + pap.LACName);
                            }
                            dbTransaction.Commit();
                            return success;
                        }
                        catch (Exception e)
                        {
                            List<string> exception = new List<string> { "Exception", e.Message };
                            return exception;
                        }
                    }
                }
            }
            else
            {
                error.Add("You don't have sufficient permissions to perform this task.");
                return error;
            }
        }

        public List<string> AddPAPProperty(PAPViewModel pap, HttpPostedFileBase[] attachments)
        {
            List<string> error = new List<string> { "Error" };
            List<string> success = new List<string> { "Success" };

            if (Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-POWER") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-LO") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-INV") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-TOPO"))
            {
                if (pap == null)
                    error.Add("Can't save an empty PAP");
                if (pap.Properties.Length < 1)
                    error.Add("Can't add a PAP without properties");
                if (string.IsNullOrWhiteSpace(pap.PresurveyorCode))
                    error.Add("Investigator Code can't be empty");
                if (string.IsNullOrWhiteSpace(pap.PresurveyorGPS))
                    error.Add("Investigator GPS tools can't be empty");
                if (pap.FileNumber < 1)
                    error.Add("PAP File number is invalide");
                if (pap.LACId < 1)
                    error.Add("LAC Id is not valid. Please refresh the page and try again");
                if (pap.PresurveyDate > DateTime.Now)
                    error.Add("Pre-Survey date can't bee in the future");

                foreach (var property in pap.Properties)
                {
                    if (string.IsNullOrWhiteSpace(property.IPointName))
                        error.Add("Can not add a property without GPS ID");
                    if (property.IPointLongitude.Equals(null))
                        error.Add("Easting value can't be empty");
                    if (property.IPointLatitude.Equals(null))
                        error.Add("Northing value can't be empty");
                    if (property.PropertyType < 1)
                        error.Add("You did not provide property");
                    if (string.IsNullOrWhiteSpace(property.Picture))
                        error.Add("Please provide Picture ID");
                    if (string.IsNullOrWhiteSpace(property.PictureName))
                        error.Add("Please upload property Picture");
                    if (string.IsNullOrWhiteSpace(property.PropertyName))
                        error.Add("Property Name can not be empty");
                    if (property.PropertyTypeName == "Tree")
                    {
                        if (property.TreeMaturity < 1)
                            error.Add("You did not provide tree maturity");
                        if (property.TreeQty < 1)
                            error.Add("Please specify tree quantity");
                    }
                    if (property.PropertyTypeName == "Culture")
                    {
                        if (property.CultureOccupationType < 1)
                            error.Add("You did not specify culture Type");
                        if (property.Surface < 1)
                            error.Add("Culture Surface can not be less than 1");
                    }
                    if (property.PropertyTypeName == "Structure")
                    {
                        if (property.StructureType < 1)
                            error.Add("Please specify Structure Type");
                        if (property.RoofType < 1)
                            error.Add("Please specify Roof Type");
                        if (property.WallType < 1)
                            error.Add("Please specify Wall Type");
                        if (property.SoilType < 1)
                            error.Add("Please specify Floor Type");
                        if (property.StructureLength < 1)
                            error.Add("Structure length can not be less than 1");
                        if (property.StructureWidth < 1)
                            error.Add("Structure width can not be less than 1");
                        if (property.RoomQty < 1)
                            error.Add("Room quantity can not be less than 1");
                        if (property.StructureUsageID < 1)
                            error.Add("Specify the valid Structure usage");
                    }
                    if (property.UserType == "U")
                    {
                        if (!property.OwnerID.HasValue)
                        {
                            if (string.IsNullOrWhiteSpace(property.OwnerFirstName))
                                error.Add("Please add the Owner's Last Name");
                            if (string.IsNullOrWhiteSpace(property.OwnerLastName))
                                error.Add("Please add the Owner's Last Name");
                            if (string.IsNullOrWhiteSpace(property.OwnerPrimaryResidenceName) || property.OwnerPrimaryResidenceId < 1)
                                error.Add("You did not choose the primary residence of the owner");
                        }
                        if (property.OwnerFileNumber == pap.FileNumber)
                            error.Add("Owner file number can not be the same as User's File Number");
                        if (property.OwnerFileNumber < 1)
                            error.Add("Please specify a valid File Number for the owner");
                    }
                }

                if (error.Count > 1)
                    return error;
                else
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        int ownerPapId = pap.PAPId;
                        try
                        {
                            var personlac = db.T_PAPLAC.FirstOrDefault(p => p.LACId == pap.LACId && p.PAPId == pap.PAPId);
                            if (personlac == null)
                            {
                                personlac = new T_PAPLAC()
                                {
                                    LACId = pap.LACId,
                                    PAPId = pap.PAPId,
                                    PresurveyDate = pap.PresurveyDate,
                                    Presurveyor = db.T_Employee.FirstOrDefault(e => e.EmployeeCode == pap.PresurveyorCode).EmployeeId,
                                    PresurveyCamera = pap.PresurveyorCamera,
                                    PresurveyGPS = pap.PresurveyorGPS,
                                    FileNumber = pap.FileNumber.Value,
                                    Created = DateTime.Now,
                                    CreatedBy = loggedUser,
                                    Updated = DateTime.Now,
                                    UpdatedBy = loggedUser
                                };
                                db.T_PAPLAC.Add(personlac);
                                db.SaveChanges();
                            }

                            foreach (var prop in pap.Properties)
                            {
                                var lacRequestId = db.T_LAC.FirstOrDefault(l => l.LACId == pap.LACId).LACRequestId;
                                var landId = db.T_Land.FirstOrDefault(l => l.LACRequestId == lacRequestId).LandId;
                                T_Point point = db.T_Point.Where(p => p.LandId == landId && p.Latitude == prop.IPointLatitude && p.Longitude == prop.IPointLongitude && p.PointName == prop.IPointName).FirstOrDefault();
                                if (point == null)
                                {
                                    point = new T_Point()
                                    {
                                        PointName = prop.IPointName,
                                        Elevation = prop.IPointElevation,
                                        Longitude = prop.IPointLongitude,
                                        Latitude = prop.IPointLatitude,
                                        LandId = landId,
                                        Created = DateTime.Now,
                                        CreatedBy = loggedUser,
                                        Updated = DateTime.Now,
                                        UpdatedBy = loggedUser
                                    };
                                    db.T_Point.Add(point);
                                    db.SaveChanges();
                                }

                                if (prop.UserType == "U")
                                {
                                    //Insert Owner in T_Person, T_PAP, T_PAPLAC, T_PAPAttachment, T_PAPLACAttachment
                                    T_Person owner = prop.OwnerID.HasValue ? db.T_Person.Find(prop.OwnerID.Value) : db.T_Person.Where(p => p.FirstName == prop.OwnerFirstName && p.LastName == prop.OwnerLastName && p.MiddleName == prop.OwnerMiddleName).FirstOrDefault();
                                    if (owner == null)
                                    {
                                        owner = new T_Person()
                                        {
                                            PersonType = "PAP",
                                            FirstName = prop.OwnerFirstName,
                                            LastName = prop.OwnerLastName,
                                            MiddleName = prop.OwnerMiddleName,
                                            Created = DateTime.Now,
                                            CreatedBy = loggedUser,
                                            Updated = DateTime.Now,
                                            UpdatedBy = loggedUser
                                        };
                                        db.T_Person.Add(owner);
                                        db.SaveChanges();
                                    }

                                    var ownerPAP = db.T_PAP.FirstOrDefault(p => p.PersonId == owner.PersonId);
                                    if (ownerPAP == null)
                                    {
                                        ownerPAP = new T_PAP()
                                        {
                                            PersonId = owner.PersonId,
                                            Picture = prop.OwnerPictureName,
                                            Created = DateTime.Now,
                                            CreatedBy = loggedUser,
                                            Updated = DateTime.Now,
                                            UpdatedBy = loggedUser
                                        };
                                        db.T_PAP.Add(ownerPAP);
                                        db.SaveChanges();
                                        ownerPapId = ownerPAP.PAPId;

                                        //Save PAP Attachment in T_Attachement and then in PAPAttachment
                                        string ownerPictureName = ownerPAP.Picture.Split('\\')[2];
                                        string ownerServerPath = folderRoot + @"PAP\" + ownerPapId.ToString() + @"\";
                                        foreach (var file in attachments)
                                        {
                                            if (file.FileName == ownerPictureName)
                                            {
                                                if (!Directory.Exists(ownerServerPath))
                                                    Directory.CreateDirectory(ownerServerPath);

                                                ownerServerPath = Path.Combine(ownerServerPath, Path.GetFileName(file.FileName));
                                                file.SaveAs(ownerServerPath);
                                                var ownerPap = db.T_PAP.Find(ownerPapId);
                                                ownerPap.Picture = ownerServerPath;
                                                db.Entry(ownerPap).State = EntityState.Modified;
                                                break;
                                            }
                                        }

                                        var ownerRes = db.T_Residence.FirstOrDefault(r => r.ResidenceAddress == prop.OwnerPrimaryResidenceName);
                                        if (ownerRes == null)
                                        {
                                            ownerRes = new T_Residence()
                                            {
                                                ResidenceAddress = pap.ResidenceName,
                                                Created = DateTime.Now,
                                                CreatedBy = loggedUser,
                                                Updated = DateTime.Now,
                                                UpdatedBy = loggedUser
                                            };
                                            db.T_Residence.Add(ownerRes);
                                            db.SaveChanges();
                                        }

                                        db.T_PAPResidence.Add(new T_PAPResidence()
                                        {
                                            PAPId = ownerPapId,
                                            ResidenceId = ownerRes.ResidenceId,
                                            IsPermanent = true,
                                            Created = DateTime.Now,
                                            CreatedBy = loggedUser,
                                            UpdateBy = loggedUser,
                                            Updated = DateTime.Now
                                        });
                                        db.SaveChanges();
                                    }

                                    ownerPapId = ownerPAP.PAPId;
                                    var paplac = db.T_PAPLAC.FirstOrDefault(p => p.PAPId == ownerPapId && p.LACId == pap.LACId);
                                    if (paplac == null)
                                    {
                                        db.T_PAPLAC.Add(new T_PAPLAC()
                                        {
                                            LACId = pap.LACId,
                                            PAPId = ownerPapId,
                                            PresurveyDate = pap.PresurveyDate,
                                            Presurveyor = db.T_Employee.FirstOrDefault(e => e.EmployeeCode == pap.PresurveyorCode).EmployeeId,
                                            PresurveyCamera = pap.PresurveyorCamera,
                                            PresurveyGPS = pap.PresurveyorGPS,
                                            FileNumber = prop.OwnerFileNumber.Value,
                                            Created = DateTime.Now,
                                            CreatedBy = loggedUser,
                                            Updated = DateTime.Now,
                                            UpdatedBy = loggedUser
                                        });
                                        db.SaveChanges();
                                    }
                                }

                                //Save property picture before property
                                string proPictureName = prop.PictureName.Split('\\')[2];
                                string proServerPath = folderRoot + @"PAP\" + ownerPapId.ToString() + @"\Properties\LAC" + pap.LACId.ToString() + @"\";
                                foreach (var file in attachments)
                                {
                                    if (file.FileName == proPictureName)
                                    {
                                        if (!Directory.Exists(proServerPath))
                                            Directory.CreateDirectory(proServerPath);

                                        proServerPath = Path.Combine(proServerPath, Path.GetFileName(file.FileName));
                                        file.SaveAs(proServerPath);
                                        break;
                                    }
                                }

                                //Insert Property
                                db.T_Property.Add(new T_Property()
                                {
                                    Owner = ownerPapId,
                                    User = pap.PAPId,
                                    LandId = landId,
                                    LACId = pap.LACId,
                                    Investigator = db.T_Employee.FirstOrDefault(e => e.EmployeeCode == pap.PresurveyorCode).EmployeeId,
                                    InvestigatorPoint = point.PointId,
                                    Picture = proServerPath,
                                    PictureName = proPictureName,
                                    PropertyType = prop.PropertyType,
                                    PropertyName = prop.PropertyName,
                                    Surface = prop.Surface,
                                    SurfaceUOM = prop.SurfaceUOM,
                                    CultureOccupationType = prop.CultureOccupationType,
                                    TreeMaturity = prop.TreeMaturity,
                                    TreeQty = prop.TreeQty,
                                    StructureType = prop.StructureType,
                                    StructureLength = prop.StructureLength,
                                    StructureWidth = prop.StructureWidth,
                                    RoomQty = prop.RoomQty,
                                    RoofType = prop.RoofType,
                                    WallType = prop.WallType,
                                    SoilType = prop.SoilType,
                                    StructureUOM = prop.StructureUOM,
                                    StructureUsage = prop.StructureUsageID,
                                    GPSDevice = pap.PresurveyorGPS,
                                    InventoryDate = (DateTime)pap.PresurveyDate,
                                    Comments = prop.Comments,
                                    Created = DateTime.Now,
                                    CreatedBy = loggedUser,
                                    Updated = DateTime.Now,
                                    UpdatedBy = loggedUser
                                });
                                db.SaveChanges();
                            }
                            scope.Complete();
                            success.Add("Properties added to LAC " + pap.LACName);
                            return success;
                        }
                        catch (Exception e)
                        {
                            try
                            {
                                string papServerPath = folderRoot + @"PAP\" + pap.PAPId.ToString() + @"\";
                                string ownerServerPath = folderRoot + @"PAP\" + ownerPapId.ToString() + @"\";

                                foreach (var file in attachments)
                                {
                                    string filePath = Directory.GetFiles(papServerPath, file.FileName, SearchOption.AllDirectories)[0];
                                    string ownerPath = Directory.GetFiles(ownerServerPath, file.FileName, SearchOption.AllDirectories)[0];
                                    if (File.Exists(filePath))
                                        File.Delete(filePath);
                                    if (File.Exists(ownerPath))
                                        File.Delete(ownerPath);
                                }
                            }
                            catch
                            {

                            }
                            List<string> exception = new List<string> { "Exception", e.Message };
                            return exception;
                        }
                    }
                }
            }
            else
            {
                error.Add("You don't have sufficient permissions to perform this task.");
                return error;
            }
        }

        public List<string> UnlinkPAPProperty(PAPViewModel pap)
        {
            List<string> error = new List<string> { "Error" };

            if (pap.Properties.Length < 1)
                error.Add("Can't add a PAP without properties");
            if (pap.PAPId < 1)
                error.Add("This PAP ID is not valid. Please refresh and try again later");
            if (pap.LACId < 1)
                error.Add("Please choose a valid LAC");

            if (error.Count > 1)
                return error;
            else
            {
                try
                {
                    var property = db.T_Property.Find(pap.Properties.FirstOrDefault().PropertyId);
                    if (property != null)
                    {
                        string picturePath = property.Picture;
                        int lacID = property.LACId;

                        if (property.User == pap.PAPId && property.Owner != pap.PAPId)
                        {
                            property.User = property.Owner;
                            property.Updated = DateTime.Now;
                            property.UpdatedBy = loggedUser;
                            db.Entry(property).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else if (property.Owner == pap.PAPId)
                        {
                            if (File.Exists(picturePath))
                                File.Delete(picturePath);
                            db.Entry(property).State = EntityState.Deleted;
                            db.SaveChanges();
                        }

                        //Check if there is no other property in the LAC and then delete PAPLAC Association
                        var lacProperty = db.T_Property.Where(p => p.LACId == lacID && (p.Owner == pap.PAPId || p.User == pap.PAPId)).ToList();
                        var paplac = db.T_PAPLAC.FirstOrDefault(p => p.LACId == lacID && p.LACId == pap.LACId);
                        if (lacProperty.Count() == 0)
                        {
                            db.T_PAPLAC.Remove(paplac);
                            db.SaveChanges();
                        }

                        List<string> success = new List<string> { "Success", "Remove successfully" };
                        return success;
                    }
                    else
                    {
                        error.Add("Either this property doesn't exists nor it is not linked to this user. Please try again later!" );
                        return error;
                    }
                }
                catch (Exception e)
                {
                    List<string> exception = new List<string> { "Exception", e.Message };
                    return exception;
                }
            }
        }

        public List<string> UnlinkPAPLAC (PAPViewModel pap)
        {
            List<string> error = new List<string> { "Error" };
            List<string> success = new List<string> { "Success" };

            if (pap.PAPId < 1)
                error.Add("Please select a valid PAP. Try to refresh the page and try again");
            if (pap.LACId < 1)
                error.Add("Please select a valid LAC. Refresh the page and try again");
            if (pap.PAPLACId < 1)
                error.Add("There is no association between this PAP " + pap.PAPId +" and LAC LAC"+pap.LACId.ToString());

            if (error.Count > 1)
                return error;
            else
            {
                try
                {
                    var paplac = db.T_PAPLAC.Find(pap.PAPLACId);
                    if (paplac != null)
                    {
                        //Get all properties for this PAP on the LAC
                        var prop = db.T_Property.Where(p => (p.Owner == paplac.PAPId || p.User == paplac.PAPId) && p.LACId == paplac.LACId).ToList();
                        if (prop.Count() > 0)
                        {
                            foreach (var property in prop)
                            {
                                string picturePath = property.Picture;
                                
                                if (property.User == pap.PAPId && property.Owner != pap.PAPId)
                                {
                                    property.User = property.Owner;
                                    property.Updated = DateTime.Now;
                                    property.UpdatedBy = loggedUser;
                                    db.Entry(property).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                                else if (property.Owner == pap.PAPId)
                                {
                                    if (File.Exists(picturePath))
                                        File.Delete(picturePath);
                                    db.Entry(property).State = EntityState.Deleted;
                                    db.SaveChanges();
                                }
                            }
                        }

                        db.Entry(paplac).State = EntityState.Deleted;
                        db.SaveChanges();
                        return success;
                    }
                    else
                    {
                        error.Add("Can not find this PAP associated to this LAC");
                        return error;
                    }
                }
                catch (Exception e)
                {
                    List<string> exception = new List<string> { "Exception", e.Message };
                    return exception;
                }
            }
        }

        public List<string> PAPLACSurvey(HouseholdViewModel hh, HttpPostedFileBase[] attachements)
        {
            List<string> error = new List<string> { "Error" };
            List<string> success = new List<string> { "Success", "PAP "+ hh.PAPs.PAPId +" successfully surveyed!" };

            if (Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-POWER") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-LO") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-INV") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-TOPO"))
            {
                if (hh == null)
                    error.Add("An error occured, please try again later. Household object is Empty");
                if (hh.PAPs == null)
                    error.Add("Please select a valid PAP. Try to refresh the page and try again");
                if (hh.PAPs.PAPId < 1)
                    error.Add("Please select a valid PAP. Try to refresh the page and try again");
                if (hh.PAPs.LACId < 1)
                    error.Add("Please select a valid LAC. Refresh the page and try again");
                if (hh.PAPs.PAPLACId < 1)
                    error.Add("There is no association between this PAP " + hh.PAPs.PAPId + " and LAC LAC" + hh.PAPs.LACId.ToString());
                if (attachements?.Count() < 1 && hh.HouseholdMembers?.Count() > 1 || hh.HouseholdMembers?.Count() > attachements?.Count())
                    error.Add("Please check attachment file. Seems like there no attachments or one Household doesn't have attachments");
                if (!hh.PAPs.SurveyDate.HasValue && hh.PAPs.SurveyDate.Value > DateTime.Today)
                    error.Add("Please specify the Survey Date");
                if (string.IsNullOrWhiteSpace(hh.PAPs.SurveyorCode))
                    error.Add("Please specify the Surveyor Code");
                if (string.IsNullOrWhiteSpace(hh.InterviewedName) && (string.IsNullOrWhiteSpace(hh.InterviewedRelationship) && hh.InterviewedRelationship == "Other"))
                    error.Add("You didn't specify the name of the Interviewed person");
                if (!string.IsNullOrWhiteSpace(hh.InterviewedName) && string.IsNullOrWhiteSpace(hh.InterviewedRelationship))
                    error.Add("Please specify the relationship of the interviewed person");
                if (!hh.NoAnimal && hh.HouseholdGoods?.Count < 1)
                    error.Add("You can not add an Household without Animal. Please check the No Animal checkbox if any");
                if (!hh.NoEquipment && hh.HouseholdGoods?.Count < 1)
                    error.Add("You can not add an Household without Equipment. Please check the No Equipment checkbox if any");
                if (!hh.NoRevenue && hh.HouseholdRevenues?.Count < 1)
                    error.Add("You can not add an Household without Revenues. Please check the No Revenue checkbox if any");
                if (!hh.PreviouslyCompensated && hh.HouseholdCompensations?.Count < 1)
                    error.Add("You can not add an Household without previous compensations. Please check the No Compensations checkbox if any");
                if (!hh.PreviouslyCompensated && string.IsNullOrWhiteSpace(hh.CompensationUse))
                    error.Add("Please specify what you did with the previous compensation amount");
                if (hh.LastWeekExpense < 1)
                    error.Add("The last week expense can't be less than 0");
                if (hh.LastWeekDeseaseName.Length < 1)
                    error.Add("Last week disease can't be null.");
                if (hh.MosquitoNetSourceName.Length > 1 && hh.MosquitoNetUserName.Length < 1)
                    error.Add("Please specify who is using Mosquitoes");
                if (hh.PaidWorkersQty > 0 && string.IsNullOrWhiteSpace(hh.PaidWorkersTime))
                    error.Add("How often are you paying workers");
                if (hh.CultureTool.Length < 1)
                    error.Add("You did not specify culture tools");

                if (error.Count > 1)
                    return error;
                else
                {
                    using (var dbTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var resp = hh.HouseholdMembers.FirstOrDefault(h => h.RelationshipName == "HH Responsible");
                            var persResp = resp != null ? db.T_Person.Where(p => p.FirstName == resp.FirstName.Trim() && p.MiddleName == resp.MiddleName.Trim() && p.LastName == resp.LastName.Trim())?.FirstOrDefault() : null;

                            if (persResp == null)
                            {
                                persResp = new T_Person()
                                {
                                    PersonType = "Household",
                                    FirstName = resp.FirstName?.Trim(),
                                    LastName = resp.LastName?.Trim(),
                                    MiddleName = resp.MiddleName?.Trim(),
                                    Mobile = resp.Mobile?.Trim(),
                                    Email = resp.Email?.Trim(),
                                    Gender = resp.Gender,
                                    IdCard = resp.IdCard?.Trim(),
                                    DateOfBirth = resp.DateOfBirth,
                                    VulnerabilityTypeId = resp.VulnerabilityTypeId,
                                    VulnerabilityDetail = resp.VulnerabilityDetail?.Trim(),
                                    Created = DateTime.Now,
                                    CreatedBy = loggedUser,
                                    Updated = DateTime.Now,
                                    UpdatedBy = loggedUser
                                };

                                db.T_Person.Add(persResp);
                                db.SaveChanges();
                            }

                            hh.HouseHoldResponsible = persResp.PersonId;

                            //check there is a residence with the residence name
                            var hRes = db.T_Residence.FirstOrDefault(r => r.ResidenceAddress == resp.ResidenceName);
                            if (hRes == null)
                            {
                                hRes = new T_Residence()
                                {
                                    ResidenceAddress = resp.ResidenceName,
                                    Created = DateTime.Now,
                                    CreatedBy = loggedUser,
                                    Updated = DateTime.Now,
                                    UpdatedBy = loggedUser
                                };
                                db.T_Residence.Add(hRes);
                                db.SaveChanges();
                            }
                            hh.ResidenceId = hRes.ResidenceId;

                            //hh.MosquitoNetUserName = string.IsNullOrWhiteSpace(hh.MosquitoNetUserName) ? "None" : hh.MosquitoNetUserName;
                            var household = new T_HouseHold()
                            {
                                HouseHoldResponsible = hh.HouseHoldResponsible,
                                HouseHoldNumber = hh.HouseHoldNumber,
                                PreviouslyCompensated = hh.PreviouslyCompensated,
                                CompensationUse = hh.CompensationUse,
                                ResidenceId = hh.ResidenceId,
                                LastWeekExpense = hh.LastWeekExpense,
                                FishOrMeat = hh.FishOrMeat,
                                EnoughFood = hh.EnoughFood,
                                SkinDesease = hh.SkinDesease,
                                PaidWorkersQty = hh.PaidWorkersQty,
                                PaidWorkersTimes = hh.PaidWorkersTime,
                                Created = DateTime.Now,
                                CreatedBy = loggedUser,
                                Updated = DateTime.Now,
                                UpdatedBy = loggedUser
                            };
                            db.T_HouseHold.Add(household);
                            db.SaveChanges();
                            hh.HouseHoldId = household.HouseHoldId;

                            //Insert Household Socio Economic elements
                            if (hh.SocioElements.Count > 0)
                            {
                                foreach (var e in hh.SocioElements)
                                {
                                    db.T_HouseHoldSocioElements.Add(new T_HouseHoldSocioElements()
                                    {
                                        SocioEconomicElements = db.T_List.FirstOrDefault(l => l.ListName == e.SocioElementName && l.ListValue == e.SocioElementValue).ListId,
                                        HouseHoldId = hh.HouseHoldId,
                                        Created = DateTime.Now,
                                        CreatedBy = loggedUser,
                                        Updated = DateTime.Now,
                                        UpdatedBy = loggedUser
                                    });
                                }
                                db.SaveChanges();
                            }

                            //Save attachement
                            if (attachements.Length > 0)
                            {
                                foreach (var f in attachements)
                                {
                                    string fileExtension = Path.GetExtension(f.FileName);
                                    if (fileExtension == ".pdf")
                                    {
                                        string serverPath = folderRoot + @"HouseHold\LAC" + hh.PAPs.LACId.ToString() + @"\" + hh.HouseHoldId.ToString() + @"\";
                                        if (!Directory.Exists(serverPath))
                                            Directory.CreateDirectory(serverPath);
                                        string fileName = Path.GetFileName(f.FileName);
                                        serverPath = Path.Combine(serverPath, fileName);

                                        var attachs = new T_Attachment()
                                        {
                                            RequestAttachementType = db.T_List.FirstOrDefault(l => l.ListName == "Attachment Type" && l.ListValue == "Household").ListId,
                                            RequestAttachementPath = serverPath,
                                            RequestAttachementContentType = f.ContentType,
                                            RequestAttachementFile = f.FileName,
                                            Created = DateTime.Now,
                                            CreatedBy = loggedUser,
                                            Updated = DateTime.Now,
                                            UpdatedBy = loggedUser
                                        };
                                        db.T_Attachment.Add(attachs);
                                        db.SaveChanges();
                                        f.SaveAs(serverPath);

                                        db.T_HouseHoldAttachment.Add(new T_HouseHoldAttachment()
                                        {
                                            HouseholdId = hh.HouseHoldId,
                                            AttachmentId = attachs.AttachmentId,
                                            Created = DateTime.Now,
                                            CreatedBy = loggedUser,
                                            Updated = DateTime.Now,
                                            UpdatedBy = loggedUser
                                        });
                                        db.SaveChanges();
                                        break;
                                    }
                                }
                            }

                            //Update T_PAP with the Household ID
                            var pap = db.T_PAP.FirstOrDefault(p => p.PAPId == hh.PAPs.PAPId);
                            pap.HouseHoldId = hh.HouseHoldId;
                            pap.Updated = DateTime.Now;
                            pap.UpdatedBy = loggedUser;
                            db.Entry(pap).State = EntityState.Modified;

                            //Insert household Members
                            if (hh.HouseholdMembers.Count > 0)
                            {
                                foreach (var member in hh.HouseholdMembers)
                                {
                                    //Check if the Member exists in Person Table
                                    var pers = db.T_Person.Where(p => p.FirstName == member.FirstName.Trim() && p.MiddleName == member.MiddleName.Trim() && p.LastName == member.LastName.Trim())?.FirstOrDefault();
                                    if (pers == null)
                                    {
                                        //NEED TO FIND HOW TO INSERT HH RESPONSIBLE AND SPOUSE PARENT WITH INCOMPLETE INFORMATION
                                        pers = new T_Person()
                                        {
                                            PersonType = "Household",
                                            FirstName = member.FirstName?.Trim(),
                                            LastName = member.LastName?.Trim(),
                                            MiddleName = member.MiddleName?.Trim(),
                                            Mobile = member.Mobile?.Trim(),
                                            Email = member.Email?.Trim(),
                                            Gender = member.Gender,
                                            IdCard = member.IdCard?.Trim(),
                                            DateOfBirth = member.DateOfBirth,
                                            VulnerabilityTypeId = member.VulnerabilityTypeId,
                                            VulnerabilityDetail = member.VulnerabilityDetail?.Trim(),
                                            Created = DateTime.Now,
                                            CreatedBy = loggedUser,
                                            Updated = DateTime.Now,
                                            UpdatedBy = loggedUser
                                        };
                                        db.T_Person.Add(pers);
                                        db.SaveChanges();
                                    }

                                    //Save HouseHold Member picture
                                    string PictureName = member.Picture.Split('\\')[2];
                                    string ServerPath = folderRoot + @"HouseHold\LAC" + hh.PAPs.LACId + @"\" + hh.HouseHoldId.ToString() + @"\Members\";
                                    foreach (var file in attachements)
                                    {
                                        if (file.FileName == PictureName)
                                        {
                                            if (!Directory.Exists(ServerPath))
                                                Directory.CreateDirectory(ServerPath);

                                            ServerPath = Path.Combine(ServerPath, Path.GetFileName(file.FileName));
                                            file.SaveAs(ServerPath);
                                            break;
                                        }
                                    }

                                    if (!string.IsNullOrWhiteSpace(member.ResidenceName))
                                    {
                                        var mRes = db.T_Residence.FirstOrDefault(r => r.ResidenceAddress == member.ResidenceName);
                                        if(mRes == null)
                                        {
                                            mRes = new T_Residence()
                                            {
                                                ResidenceAddress = member.ResidenceName,
                                                Created = DateTime.Now,
                                                CreatedBy = loggedUser,
                                                Updated = DateTime.Now,
                                                UpdatedBy = loggedUser
                                            };
                                            db.T_Residence.Add(mRes);
                                            db.SaveChanges();
                                        }
                                        member.ResidenceId = mRes.ResidenceId;

                                        //Insert into Household Members table
                                        db.T_HouseHoldMembers.Add(new T_HouseHoldMembers()
                                        {
                                            PersonId = pers.PersonId,
                                            HouseholdId = hh.HouseHoldId,
                                            MaritalStatus = member.MaritalStatus.Substring(0, 1),
                                            Relationship = member.Relationship,
                                            IsStudent = member.IsStudent,
                                            EducationLevel = member.EducationLevel,
                                            FrenchLevel = member.FrenchLevel,
                                            Picture = ServerPath,
                                            ResidenceId = member.ResidenceId,
                                            Activity1 = member.Activity1,
                                            Activity2 = member.Activity2,
                                            Competency1 = member.Competency1,
                                            Competency2 = member.Competency2,
                                            Created = DateTime.Now,
                                            CreatedBy = loggedUser,
                                            Updated = DateTime.Now,
                                            UpdatedBy = loggedUser
                                        });
                                        db.SaveChanges();
                                    }
                                }
                            }

                            //Insert Household Goods
                            if (!hh.NoAnimal || !hh.NoEquipment)
                            {
                                if (hh.HouseholdGoods.Count > 0)
                                {
                                    foreach (var good in hh.HouseholdGoods)
                                    {
                                        db.T_HouseHoldGood.Add(new T_HouseHoldGood()
                                        {
                                            HouseholdId = hh.HouseHoldId,
                                            Good = good.Good,
                                            GoodName = good.GoodDescription,
                                            Quantity = good.Quantity,
                                            QuantityUOM = db.T_UOM.FirstOrDefault(u => u.UOM == "U").UOMId,
                                            Created = DateTime.Now,
                                            CreatedBy = loggedUser,
                                            Updated = DateTime.Now,
                                            UpdatedBy = loggedUser
                                        });
                                    }
                                    db.SaveChanges();
                                }
                            }

                            //Insert HouseHold Revenue
                            if (!hh.NoRevenue)
                            {
                                if (hh.HouseholdRevenues.Count > 0)
                                {
                                    foreach (var revenue in hh.HouseholdRevenues)
                                    {
                                        db.T_HouseHoldRevenue.Add(new T_HouseHoldRevenue()
                                        {
                                            HouseHoldId = hh.HouseHoldId,
                                            RevenueSource = revenue.RevenueSource,
                                            RevenueAmount = revenue.RevenueAmount,
                                            MonthsPerYear = revenue.MonthsPerYear,
                                            Created = DateTime.Now,
                                            CreatedBy = loggedUser,
                                            Updated = DateTime.Now,
                                            UpdatedBy = loggedUser
                                        });
                                    }
                                    db.SaveChanges();
                                }
                            }

                            //Insert Household Previous Compensation
                            if (!hh.PreviouslyCompensated)
                            {
                                if (hh.HouseholdCompensations.Count > 0)
                                {
                                    foreach (var comp in hh.HouseholdCompensations)
                                    {
                                        db.T_HouseHoldCompensation.Add(new T_HouseHoldCompensation()
                                        {
                                            HouseHoldId = hh.HouseHoldId,
                                            Year = comp.Year,
                                            Amount = comp.Amount,
                                            Area = comp.Surface,
                                            AreaUOM = db.T_UOM.FirstOrDefault(u => u.UOM == "25m2" && u.UOMType == 33).UOMId,
                                            Created = DateTime.Now,
                                            CreatedBy = loggedUser,
                                            Updated = DateTime.Now,
                                            UpdatedBy = loggedUser
                                        });
                                    }
                                    db.SaveChanges();
                                }
                            }

                            //Insert HouseHold Culture Tools
                            if (hh.CultureTool.Length > 0)
                            {
                                foreach (var item in hh.CultureTool)
                                {
                                    db.T_HouseHoldCultureTool.Add(new T_HouseHoldCultureTool()
                                    {
                                        HouseholdId = hh.HouseHoldId,
                                        CultureToolId = item,
                                        Created = DateTime.Now,
                                        CreatedBy = loggedUser,
                                        Updated = DateTime.Now,
                                        UpdatedBy = loggedUser
                                    });
                                }
                                db.SaveChanges();
                            }

                            //Insert HouseHold Properties
                            if (hh.HouseholdProperties.Count > 0)
                            {
                                foreach (var prop in hh.HouseholdProperties)
                                {
                                    var lac = db.T_LAC.FirstOrDefault(l => l.LACId == hh.PAPs.LACId).LACRequestId;

                                    var hhLocation = db.T_Location.FirstOrDefault(l=>l.LocationName == prop.LandName && l.RegionId == prop.RegionId);
                                    if(hhLocation == null)
                                    {
                                        hhLocation = new T_Location()
                                        {
                                            LocationName = prop.LandName,
                                            RegionId = prop.RegionId,
                                            CreatedBy = loggedUser,
                                            UpdateBy = loggedUser
                                        };
                                        db.T_Location.Add(hhLocation);
                                        db.SaveChanges();
                                    }

                                    var hhLand = db.T_Land.FirstOrDefault(l=>l.LandName == prop.LandName);
                                    if(hhLand == null)
                                    {
                                        hhLand = new T_Land()
                                        {
                                            LandName = prop.LandName,
                                            LandCategory = db.T_List.FirstOrDefault(l=>l.ListName == "Land Category" && l.ListValue == "Property").ListId,
                                            LACRequestId = lac,
                                            LandStatus = db.T_List.FirstOrDefault(l=>l.ListName == "Land Status" && l.ListValue == "Active").ListId,
                                            LocationId = hhLocation.LocationId,
                                            GPS_Date = DateTime.Now,
                                            Created = DateTime.Now,
                                            CreatedBy = loggedUser,
                                            Updated = DateTime.Now,
                                            UpdatedBy = loggedUser
                                        };
                                        db.T_Land.Add(hhLand);
                                        db.SaveChanges();
                                    }

                                    var property = new T_HouseHoldProperty()
                                    {
                                        HouseholdId = hh.HouseHoldId,
                                        LandId = hhLand.LandId,
                                        DistanceFromResidence = prop.DistanceFromResidence,
                                        DistanceUOM = db.T_UOM.FirstOrDefault(u => u.UOM == "min").UOMId,
                                        CultivatedArea = prop.CultivatedArea,
                                        CultivatedAreaUOM = prop.CultivatedArea.HasValue ? db.T_UOM.FirstOrDefault(u => u.UOM == "25m2" && u.UOMType == 33).UOMId : default(int?),
                                        FallowArea = prop.FallowArea,
                                        FallowAreaUOM = prop.FallowArea.HasValue ? db.T_UOM.FirstOrDefault(u => u.UOM == "25m2" && u.UOMType == 33).UOMId : default(int?),
                                        IsOwner = prop.IsOwner,
                                        Rent = prop.Rent,
                                        PropertySource = db.T_List.FirstOrDefault(l => l.ListName == "Property Source" && l.ListValue == prop.PropertySourceName).ListId,
                                        AcquisitionYear = prop.AcquisitionYear,
                                        IsCultivatedLastYear = (prop.CultivatedArea > 0),
                                        Created = DateTime.Now,
                                        CreatedBy = loggedUser,
                                        Updated = DateTime.Now,
                                        UpdatedBy = loggedUser
                                    };
                                    db.T_HouseHoldProperty.Add(property);
                                    db.SaveChanges();

                                    if (prop.PropertyWorkerType.Length > 0)
                                    {
                                        foreach (var i in prop.PropertyWorkerType)
                                        {
                                            db.T_HouseHoldPropertyWorker.Add(new T_HouseHoldPropertyWorker()
                                            {
                                                HouseHoldPropertyId = property.HouseHoldPropertyId,
                                                WorkerId = i,
                                                Created = DateTime.Now,
                                                CreatedBy = loggedUser,
                                                Updated = DateTime.Now,
                                                UpdatedBy = loggedUser
                                            });
                                        }
                                        db.SaveChanges();
                                    }
                                }
                            }

                            //Insert Household Residence
                            if (hh.HouseholdResidences.Count > 0)
                            {
                                foreach (var res in hh.HouseholdResidences)
                                {
                                    var r = db.T_Residence.FirstOrDefault(rs => rs.ResidenceAddress == res.ResidenceAddress);

                                    if (r == null)
                                    {
                                        r = new T_Residence()
                                        {
                                            ResidenceAddress = res.ResidenceAddress,
                                            Created = DateTime.Now,
                                            CreatedBy = loggedUser,
                                            Updated = DateTime.Now,
                                            UpdatedBy = loggedUser
                                        };
                                        db.T_Residence.Add(r);
                                        db.SaveChanges();
                                    }

                                    db.T_HouseHoldResidence.Add(new T_HouseHoldResidence()
                                    {
                                        HouseholdId = hh.HouseHoldId,
                                        ResidenceId = r.ResidenceId,
                                        RoofType = res.RoofType,
                                        WallType = res.WallType,
                                        SoilType = res.FloorType,
                                        RoomQty = res.RoomQty,
                                        IsOwner = res.IsOwner,
                                        Rent = res.Rent,
                                        ArrivalDate = res.ArrivalDate,
                                        ArrivalReason = res.ArrivalReason,
                                        ToiletType = res.ToiletType,
                                        IsPermanent = res.IsPermanent,
                                        Created = DateTime.Now,
                                        CreatedBy = loggedUser,
                                        Updated = DateTime.Now,
                                        UpdatedBy = loggedUser
                                    });
                                    db.SaveChanges();
                                }
                            }

                            //Insert Household Cultures
                            if (hh.HouseholdCultures.Count > 0)
                            {
                                foreach (var cult in hh.HouseholdCultures)
                                {
                                    var clt = db.T_HouseHoldCulture.FirstOrDefault(c => c.HouseholdId == hh.HouseHoldId && c.CultureDiversityId == cult.CultureDiversityId);

                                    if (clt == null)
                                    {
                                        clt = new T_HouseHoldCulture()
                                        {
                                            HouseholdId = hh.HouseHoldId,
                                            Area = cult.Area ?? 0,
                                            AreaUOM = db.T_UOM.FirstOrDefault(u => u.UOM == "25m2" && u.UOMType == 33).UOMId,
                                            CultureDiversityId = cult.CultureDiversityId,
                                            PaidWorkersQty = cult.PaidWorkersQty,
                                            IsPurchased = cult.IsPurchased,
                                            Created = DateTime.Now,
                                            CreatedBy = loggedUser,
                                            Updated = DateTime.Now,
                                            UpdatedBy = loggedUser
                                        };
                                        db.T_HouseHoldCulture.Add(clt);
                                        db.SaveChanges();
                                    }

                                    if (!string.IsNullOrWhiteSpace(cult.CultureAction))
                                    {
                                        var cav = cult.CultureAction.Split(',');
                                        foreach (var i in cav)
                                        {
                                            cult.CultureActionType = db.T_List.FirstOrDefault(l => l.ListName == "CAV" && l.ListValue == i).ListId;
                                            var clm = db.T_HouseHoldCultureLastMonths.FirstOrDefault(c => c.HouseholdCultureId == clt.HouseholdCultureId);
                                            if (clm == null)
                                            {
                                                clm = new T_HouseHoldCultureLastMonths()
                                                {
                                                    HouseholdCultureId = clt.HouseholdCultureId,
                                                    CultureActionType = cult.CultureActionType.Value,
                                                    Created = DateTime.Now,
                                                    CreatedBy = loggedUser,
                                                    Updated = DateTime.Now,
                                                    UpdatedBy = loggedUser
                                                };
                                                db.T_HouseHoldCultureLastMonths.Add(clm);
                                            }
                                            else
                                            {
                                                clm.CultureActionType = cult.CultureActionType.Value;
                                                clm.Updated = DateTime.Now;
                                                clm.UpdatedBy = loggedUser;
                                                db.Entry(clm).State = EntityState.Modified;
                                            }
                                        }
                                        db.SaveChanges();
                                    }

                                    if (cult.Area > 0 && cult.HarvestLastSeason > 0)
                                    {
                                        db.T_HouseHoldCultureLastSeason.Add(new T_HouseHoldCultureLastSeason()
                                        {
                                            HouseholdCultureId = clt.HouseholdCultureId,
                                            HarvestLastSeason = cult.HarvestLastSeason,
                                            HarvestUOM = cult.HarvestLastSeason.HasValue ? db.T_UOM.FirstOrDefault(u => u.UOM == "Meka" && u.UOMType == 35).UOMId : default(int?),
                                            SoldLastSeason = cult.SoldLastSeason,
                                            SoldUOM = cult.SoldLastSeason.HasValue ? db.T_UOM.FirstOrDefault(u => u.UOM == "Meka" && u.UOMType == 35).UOMId : default(int?),
                                            Revenue = cult.Revenue,
                                            RevenueUOM = cult.Revenue.HasValue ? db.T_UOM.FirstOrDefault(u => u.UOM == "K Fc").UOMId : default(int?),
                                            Created = DateTime.Now,
                                            CreatedBy = loggedUser,
                                            Updated = DateTime.Now,
                                            UpdatedBy = loggedUser
                                        });
                                        db.SaveChanges();
                                    }
                                }
                            }

                            //Update T_PAPLAC with Survey Information
                            var paplac = db.T_PAPLAC.FirstOrDefault(p => p.PAPLACId == hh.PAPs.PAPLACId);
                            paplac.Surveyor = db.T_Employee.FirstOrDefault(e => e.EmployeeCode == hh.PAPs.SurveyorCode).EmployeeId;
                            paplac.SurveyDate = hh.PAPs.SurveyDate;
                            paplac.SurveyCamera = hh.PAPs.PresurveyorCamera;
                            paplac.SurveyGPS = hh.PAPs.PresurveyorGPS;
                            paplac.Updated = DateTime.Now;
                            paplac.UpdatedBy = loggedUser;
                            db.Entry(paplac).State = EntityState.Modified;

                            dbTransaction.Commit();
                            return success;
                        }
                        catch (Exception e)
                        {
                            List<string> exception = new List<string> { "Exception", e.Message };
                            try
                            {
                                dbTransaction.Rollback();
                            }
                            finally
                            {
                                string serverPath = folderRoot + @"HouseHold\LAC" + hh.PAPs.LACId.ToString() + @"\" + hh.HouseHoldId.ToString() + @"\";
                                if (Directory.Exists(serverPath))
                                    Directory.Delete(serverPath, true);
                            }
                            return exception;
                        }
                    }
                }
            }
            else
            {
                error.Add("You don't have sufficient permissions to perform this task.");
                return error;
            }
        }

        public List<string> CollectLandData(PAPViewModel pap, HttpPostedFileBase[] attachments)
        {
            List<string> error = new List<string> { "Error" };
            List<string> success = new List<string> { "Success", "Land data saved successfully saved" };

            if (Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-POWER") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-LO") ||  Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-TOPO"))
            {
                if (pap == null)
                    error.Add("Can't save an empty PAP");
                if (pap.PAPId < 1)
                    error.Add("Please select a valid PAP. Try to refresh the page and try again");
                if (pap.LACId < 1)
                    error.Add("Please select a valid LAC. Refresh the page and try again");
                if (pap.Properties.Length < 1)
                    error.Add("Can't update a PAP without properties");

                if (error.Count > 1)
                    return error;
                else
                {
                    using (var dbTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            string oldValue, newValue, logAction;
                            oldValue = null;
                            db.Configuration.ProxyCreationEnabled = false;

                            JsonSerializerSettings settings = new JsonSerializerSettings
                            {
                                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                                Formatting = Formatting.None
                            };

                            //Update Person data
                            string spouseId = !string.IsNullOrWhiteSpace(pap.SpouseName) ? (pap.SpouseName?.Split('(')[1])?.Substring(0, (int)(pap.SpouseName?.Split('(')[1])?.Length - 1) : null;
                            string motherId = !string.IsNullOrWhiteSpace(pap.MotherName) ? (pap.MotherName?.Split('(')[1])?.Substring(0, (int)(pap.MotherName?.Split('(')[1])?.Length - 1) : null;
                            string fatherId = !string.IsNullOrWhiteSpace(pap.FatherName) ? (pap.FatherName?.Split('(')[1])?.Substring(0, (int)(pap.FatherName?.Split('(')[1])?.Length - 1) : null;
                            int? spouse = !string.IsNullOrWhiteSpace(spouseId) ? int.Parse(spouseId) : default(int?);
                            int? mother = !string.IsNullOrWhiteSpace(motherId) ? int.Parse(motherId) : default(int?);
                            int? father = !string.IsNullOrWhiteSpace(fatherId) ? int.Parse(fatherId) : default(int?);

                            var person = pap.PersonId > 1 ? db.T_Person.FirstOrDefault(p => p.PersonId == pap.PersonId) : db.T_Person.FirstOrDefault(p => p.FirstName == pap.FirstName && p.LastName == pap.LastName && p.MiddleName == pap.MiddleName);

                            int defaultVulnerability = db.T_List.FirstOrDefault(l => l.ListName == "Vulnerability" && l.ListValue == "None").ListId;
                            pap.VulnerabilityTypeId = pap.VulnerabilityTypeId == -1 ? defaultVulnerability : pap.VulnerabilityTypeId;

                            if (person == null)
                            {
                                person = new T_Person()
                                {
                                    PersonType = "PAP",
                                    FirstName = pap.FirstName,
                                    LastName = pap.LastName,
                                    MiddleName = pap.MiddleName,
                                    Mobile = pap.Mobile,
                                    Email = pap.Email,
                                    Gender = pap.Gender,
                                    IdCard = pap.IdCard,
                                    DateOfBirth = pap.DateOfBirth,
                                    PlaceOfBirth = pap.PlaceOfBirth,
                                    Father = father,
                                    Mother = mother,
                                    VulnerabilityTypeId = pap.VulnerabilityTypeId,
                                    VulnerabilityDetail = pap.VulnerabilityDetail,
                                    Created = DateTime.Now,
                                    CreatedBy = loggedUser,
                                    Updated = DateTime.Now,
                                    UpdatedBy = loggedUser
                                };
                                db.T_Person.Add(person);
                                logAction = "Insert";
                            }
                            else
                            {
                                oldValue = JsonConvert.SerializeObject(person, settings);
                                person.FirstName = pap.FirstName;
                                person.LastName = pap.LastName;
                                person.MiddleName = pap.MiddleName;
                                person.Mobile = pap.Mobile;
                                person.Email = pap.Email;
                                person.Gender = pap.Gender;
                                person.IdCard = pap.IdCard;
                                person.DateOfBirth = pap.DateOfBirth;
                                person.PlaceOfBirth = pap.PlaceOfBirth;
                                person.Father = father;
                                person.Mother = mother;
                                person.VulnerabilityTypeId = pap.VulnerabilityTypeId;
                                person.VulnerabilityDetail = pap.VulnerabilityDetail;
                                person.Updated = DateTime.Now;
                                person.UpdatedBy = loggedUser;
                                db.Entry(person).State = EntityState.Modified;
                                logAction = "Update";
                            }
                            db.SaveChanges();
                            pap.PersonId = person.PersonId;

                            newValue = JsonConvert.SerializeObject(person, settings);
                            db.T_Log.Add(new T_Log()
                            {
                                Person_ID = loggedUser,
                                Action = logAction,
                                Table_Name = "T_Person",
                                Table_Reference_ID = person.PersonId,
                                OldValue = oldValue,
                                NewValue = newValue,
                                LoggedDate = DateTime.Now
                            });

                            //Save PAP in PAP Table after checking if the pap already exists
                            oldValue = null;
                            var existingPAP = db.T_PAP.Find(pap.PAPId);
                            if (existingPAP == null)
                            {
                                existingPAP = new T_PAP()
                                {
                                    PersonId = person.PersonId,
                                    Picture = pap.Picture,
                                    Spouse = spouse,
                                    IdCard = pap.IdCard,
                                    Created = DateTime.Now,
                                    CreatedBy = loggedUser,
                                    Updated = DateTime.Now,
                                    UpdatedBy = loggedUser
                                };
                                db.T_PAP.Add(existingPAP);
                                logAction = "Insert";
                            }
                            else
                            {
                                oldValue = JsonConvert.SerializeObject(existingPAP, settings);
                                logAction = "Update";
                                existingPAP.Spouse = spouse;
                                existingPAP.IdCard = pap.IdCard;
                                existingPAP.Updated = DateTime.Now;
                                existingPAP.UpdatedBy = loggedUser;
                                db.Entry(existingPAP).State = EntityState.Modified;
                            }
                            db.SaveChanges();
                            pap.PAPId = existingPAP.PAPId;
                            newValue = JsonConvert.SerializeObject(existingPAP, settings);

                            var papLac = db.T_PAPLAC.FirstOrDefault(p=>p.PAPId == pap.PAPId && p.LACId == pap.LACId);
                            if(papLac == null)
                            {
                                papLac = new T_PAPLAC()
                                {
                                    LACId = pap.LACId,
                                    PAPId = pap.PAPId,
                                    PresurveyDate = pap.PresurveyDate,
                                    Presurveyor = db.T_Employee.FirstOrDefault(e => e.EmployeeCode == pap.PresurveyorCode).EmployeeId,
                                    PresurveyGPS = pap.PresurveyorGPS,
                                    PresurveyCamera = pap.PresurveyorCamera,
                                    FileNumber = pap.FileNumber.Value,
                                    Created = DateTime.Now,
                                    CreatedBy = loggedUser,
                                    Updated = DateTime.Now,
                                    UpdatedBy = loggedUser
                                };
                                db.T_PAPLAC.Add(papLac);
                                db.SaveChanges();
                            }

                            pap.PAPLACId = papLac.PAPLACId;

                            //Save PAP Picture
                            if (pap.PAPFile != null && pap.PAPFile?.Length > 0)
                            {
                                string pictureName = pap.Picture.Split('\\')[2];
                                string papServerPath = folderRoot + @"PAP\" + pap.PAPId.ToString() + @"\";
                                foreach (var file in pap.PAPFile)
                                {
                                    if (file.FileName == pictureName)
                                    {
                                        if (!Directory.Exists(papServerPath))
                                            Directory.CreateDirectory(papServerPath);

                                        if (File.Exists(existingPAP.Picture))
                                            File.Delete(existingPAP.Picture);

                                        papServerPath = Path.Combine(papServerPath, Path.GetFileName(file.FileName));
                                        file.SaveAs(papServerPath);
                                        var currentPap = db.T_PAP.Find(pap.PAPId);
                                        oldValue = JsonConvert.SerializeObject(currentPap, settings);
                                        currentPap.Picture = papServerPath;
                                        db.Entry(currentPap).State = EntityState.Modified;
                                        newValue = JsonConvert.SerializeObject(currentPap, settings);
                                        break;
                                    }
                                }
                            }

                            //Save PAPLAC Trimble File
                            if (attachments.Length > 0)
                            {
                                foreach (var f in attachments)
                                {
                                    string fileExtension = Path.GetExtension(f.FileName);
                                    if (fileExtension == ".pdf" || fileExtension == ".pos")
                                    {
                                        string serverPath = folderRoot + @"PAP\" + pap.PAPId.ToString() + @"\Properties\LAC" + pap.LACId.ToString() + @"\";

                                        if (!Directory.Exists(serverPath))
                                            Directory.CreateDirectory(serverPath);
                                        string fileName = Path.GetFileName(f.FileName);
                                        serverPath = Path.Combine(serverPath, fileName);

                                        var attachs = new T_Attachment()
                                        {
                                            RequestAttachementType = db.T_List.FirstOrDefault(l => l.ListName == "Attachment Type" && l.ListValue == "PAPLAC").ListId,
                                            RequestAttachementPath = serverPath,
                                            RequestAttachementContentType = f.ContentType,
                                            RequestAttachementFile = f.FileName,
                                            Created = DateTime.Now,
                                            CreatedBy = loggedUser,
                                            Updated = DateTime.Now,
                                            UpdatedBy = loggedUser
                                        };
                                        db.T_Attachment.Add(attachs);
                                        db.SaveChanges();
                                        f.SaveAs(serverPath);

                                        db.T_PAPLACAttachment.Add(new T_PAPLACAttachment() { 
                                            PAPLACId = pap.PAPLACId,
                                            AttachmentId = attachs.AttachmentId,
                                            Created = DateTime.Now,
                                            CreatedBy = loggedUser,
                                            Updated = DateTime.Now,
                                            UpdatedBy = loggedUser
                                        });
                                        db.SaveChanges();
                                    }
                                }
                            }

                            db.T_Log.Add(new T_Log()
                            {
                                Person_ID = loggedUser,
                                Action = logAction,
                                Table_Name = "T_PAP",
                                Table_Reference_ID = existingPAP.PAPId,
                                OldValue = oldValue,
                                NewValue = newValue,
                                LoggedDate = DateTime.Now
                            });

                            var residence = db.T_Residence.FirstOrDefault(r => r.ResidenceAddress == pap.ResidenceName);
                            if (residence == null)
                            {
                                residence = new T_Residence()
                                {
                                    ResidenceAddress = pap.ResidenceName,
                                    Created = DateTime.Now,
                                    CreatedBy = loggedUser,
                                    Updated = DateTime.Now,
                                    UpdatedBy = loggedUser
                                };
                                db.T_Residence.Add(residence);
                            }
                            else
                            {
                                residence.ResidenceAddress = pap.ResidenceName;
                                residence.Updated = DateTime.Now;
                                residence.UpdatedBy = loggedUser;
                                db.Entry(residence).State = EntityState.Modified;
                            }
                            db.SaveChanges();

                            var papRes = db.T_PAPResidence.FirstOrDefault(r => r.PAPId == pap.PAPId && r.ResidenceId == residence.ResidenceId);
                            if (papRes == null)
                            {
                                db.T_PAPResidence.Add(new T_PAPResidence()
                                {
                                    PAPId = pap.PAPId,
                                    ResidenceId = residence.ResidenceId,
                                    IsPermanent = true,
                                    Created = DateTime.Now,
                                    CreatedBy = loggedUser,
                                    Updated = DateTime.Now,
                                    UpdateBy = loggedUser
                                });
                            }
                            else
                            {
                                papRes.IsPermanent = true;
                                papRes.Updated = DateTime.Now;
                                papRes.UpdateBy = loggedUser;
                                db.Entry(papRes).State = EntityState.Modified;

                                var residences = db.T_PAPResidence.Where(r => r.PAPId == pap.PAPId && r.IsPermanent && r.ResidenceId != residence.ResidenceId);
                                residences?.ToList().ForEach(r => r.IsPermanent = false);
                            }

                            db.SaveChanges();

                            //Update propertys Info
                            var lacRequestId = db.T_LAC.FirstOrDefault(l => l.LACId == pap.LACId).LACRequestId;
                            var landId = db.T_Land.FirstOrDefault(l => l.LACRequestId == lacRequestId).LandId;
                            int ownerPapId = pap.PAPId;
                            if (pap.PAPId > 1)
                            {
                                foreach (var prop in pap.Properties)
                                {
                                    prop.TPointLongitude = prop.TPointLongitude ?? prop.IPointLongitude;
                                    prop.TPointLatitude = prop.TPointLatitude ?? prop.IPointLatitude;
                                    
                                    T_Point point = db.T_Point.Where(p => p.LandId == landId && p.Latitude == prop.IPointLatitude && p.Longitude == prop.IPointLongitude && p.PointName == prop.TPointName).FirstOrDefault();
                                    if (point == null)
                                    {
                                        point = new T_Point()
                                        {
                                            PointName = prop.TPointName,
                                            Elevation = prop.IPointElevation,
                                            Longitude = prop.IPointLongitude,
                                            Latitude = prop.IPointLatitude,
                                            LandId = landId,
                                            Created = DateTime.Now,
                                            CreatedBy = loggedUser,
                                            Updated = DateTime.Now,
                                            UpdatedBy = loggedUser
                                        };
                                        db.T_Point.Add(point);
                                        db.SaveChanges();
                                    }

                                    T_Point innerPoint = db.T_Point.Where(p => p.LandId == landId && p.Latitude == prop.TPointLatitude && p.Longitude == prop.TPointLongitude && p.PointName == prop.IPointName).FirstOrDefault();
                                    if (prop.PropertyTypeName == "Culture")
                                    {
                                        if (innerPoint == null)
                                        {
                                            innerPoint = new T_Point()
                                            {
                                                PointName = prop.IPointName,
                                                Elevation = prop.TPointElevation,
                                                Longitude = prop.TPointLongitude.Value,
                                                Latitude = prop.TPointLatitude.Value,
                                                LandId = landId,
                                                Created = DateTime.Now,
                                                CreatedBy = loggedUser,
                                                Updated = DateTime.Now,
                                                UpdatedBy = loggedUser
                                            };
                                            db.T_Point.Add(innerPoint);
                                            db.SaveChanges();
                                        }
                                    }

                                    if (prop.UserType == "U" || prop.UserType == "P")
                                    {
                                        //Insert Owner in T_Person, T_PAP, T_PAPLAC, T_PAPAttachment, T_PAPLACAttachment
                                        T_Person owner = prop.OwnerID.HasValue ? db.T_PAP.Find(prop.OwnerID.Value)?.T_Person : db.T_Person.Where(p => p.FirstName == prop.OwnerFirstName && p.LastName == prop.OwnerLastName && p.MiddleName == prop.OwnerMiddleName).FirstOrDefault();
                                        if (owner == null)
                                        {
                                            owner = new T_Person()
                                            {
                                                PersonType = "PAP",
                                                FirstName = prop.OwnerFirstName,
                                                LastName = prop.OwnerLastName,
                                                MiddleName = prop.OwnerMiddleName,
                                                Created = DateTime.Now,
                                                CreatedBy = loggedUser,
                                                Updated = DateTime.Now,
                                                UpdatedBy = loggedUser
                                            };
                                            db.T_Person.Add(owner);
                                            db.SaveChanges();
                                        }

                                        var ownerPAP = prop.OwnerID.HasValue ? db.T_PAP.Find(prop.OwnerID.Value) : db.T_PAP.Where(p => p.PersonId == owner.PersonId).FirstOrDefault();
                                        if (ownerPAP == null)
                                        {
                                            ownerPAP = new T_PAP()
                                            {
                                                PersonId = owner.PersonId,
                                                Picture = prop.OwnerPictureName,
                                                Created = DateTime.Now,
                                                CreatedBy = loggedUser,
                                                Updated = DateTime.Now,
                                                UpdatedBy = loggedUser
                                            };
                                            db.T_PAP.Add(ownerPAP);
                                            db.SaveChanges();
                                        }

                                        ownerPapId = prop.UserType == "U" ? (prop.OwnerID ?? ownerPAP.PAPId) : pap.PAPId;
                                        var paplac = db.T_PAPLAC.FirstOrDefault(p => p.PAPId == ownerPapId && p.LACId == pap.LACId);
                                        if (paplac == null)
                                        {
                                            db.T_PAPLAC.Add(new T_PAPLAC()
                                            {
                                                LACId = pap.LACId,
                                                PAPId = ownerPapId,
                                                PresurveyDate = pap.PresurveyDate,
                                                Presurveyor = db.T_Employee.FirstOrDefault(e => e.EmployeeCode == pap.PresurveyorCode).EmployeeId,
                                                PresurveyGPS = pap.PresurveyorGPS,
                                                PresurveyCamera = pap.PresurveyorCamera,
                                                FileNumber = prop.OwnerFileNumber.Value,
                                                Created = DateTime.Now,
                                                CreatedBy = loggedUser,
                                                Updated = DateTime.Now,
                                                UpdatedBy = loggedUser
                                            });
                                            db.SaveChanges();
                                        }
                                    }

                                    //Save property picture before property
                                    string proPictureName = prop.PictureName.Split('\\')[2];
                                    string proServerPath = folderRoot + @"PAP\" + (prop.UserType == "U" ? ownerPapId.ToString() : pap.PAPId.ToString()) + @"\Properties\LAC" + pap.LACId.ToString() + @"\";
                                    foreach (var file in attachments)
                                    {
                                        if (file.FileName == proPictureName)
                                        {
                                            if (!Directory.Exists(proServerPath))
                                                Directory.CreateDirectory(proServerPath);

                                            proServerPath = Path.Combine(proServerPath, Path.GetFileName(file.FileName));
                                            file.SaveAs(proServerPath);
                                            break;
                                        }
                                    }

                                    //Insert Property
                                    oldValue = null;
                                    var property = db.T_Property.Find(prop.PropertyId);
                                    if (property == null)
                                    {
                                        property = new T_Property()
                                        {
                                            Owner = (prop.UserType == "P" || prop.UserType == "PU") ? pap.PAPId : ownerPapId,
                                            User = prop.UserType == "U" ? pap.PAPId : ownerPapId,
                                            LandId = landId,
                                            LACId = pap.LACId,
                                            Investigator = db.T_Employee.FirstOrDefault(e => e.EmployeeCode == prop.TopographerCode).EmployeeId,
                                            InvestigatorPoint = point.PointId,
                                            Picture = proServerPath,
                                            PictureName = proPictureName,
                                            PropertyType = prop.PropertyType,
                                            PropertyName = prop.PropertyName,
                                            Surface = prop.Surface,
                                            SurfaceUOM = prop.SurfaceUOM,
                                            CultureOccupationType = prop.CultureOccupationType,
                                            CultureInnerPoint = innerPoint?.PointId,
                                            CultureStartDate = prop.CultureStartDate,
                                            TreeMaturity = prop.TreeMaturity,
                                            TreeQty = prop.TreeQty,
                                            StructureType = prop.StructureType,
                                            StructureLength = prop.StructureLength,
                                            StructureWidth = prop.StructureWidth,
                                            RoomQty = prop.RoomQty,
                                            RoofType = prop.RoofType,
                                            WallType = prop.WallType,
                                            SoilType = prop.SoilType,
                                            StructureUOM = db.T_UOM.FirstOrDefault(u => u.UOM == "m" && u.UOMType == 33).UOMId,
                                            StructureUsage = prop.StructureUsageID,
                                            GPSDevice = pap.PresurveyorGPS,
                                            InventoryDate = (DateTime)pap.PresurveyDate,
                                            CameraCode = pap.PresurveyorCamera,
                                            MeasurementDate = prop.TopographDate,
                                            MeasurementSignatureDate = prop.TopographDate,
                                            TrimbleCode = prop.TrimbleCode,
                                            TrimbleFile = prop.TrimbleFile,
                                            TopographerId = db.T_Employee.FirstOrDefault(e => e.EmployeeCode == prop.TopographerCode).EmployeeId,
                                            TopographerPoint = point.PointId,
                                            Comments = prop.Comments,
                                            Created = DateTime.Now,
                                            CreatedBy = loggedUser,
                                            Updated = DateTime.Now,
                                            UpdatedBy = loggedUser
                                        };
                                        db.T_Property.Add(property);
                                        logAction = "Insert";
                                    }
                                    else
                                    {
                                        oldValue = JsonConvert.SerializeObject(property, settings);
                                        logAction = "Update";
                                        property.Owner = (prop.UserType == "P" || prop.UserType == "PU") ? pap.PAPId : ownerPapId;
                                        property.User = prop.UserType == "U" ? pap.PAPId : ownerPapId;
                                        property.Picture = proServerPath;
                                        property.PictureName = proPictureName;
                                        property.PropertyType = prop.PropertyType;
                                        property.PropertyName = prop.PropertyName;
                                        property.Surface = prop.Surface;
                                        property.SurfaceUOM = prop.SurfaceUOM;
                                        property.CultureOccupationType = prop.CultureOccupationType;
                                        property.CultureInnerPoint = innerPoint?.PointId;
                                        property.TreeMaturity = prop.TreeMaturity;
                                        property.TreeQty = prop.TreeQty;
                                        property.StructureType = prop.StructureType;
                                        property.StructureLength = prop.StructureLength;
                                        property.StructureWidth = prop.StructureWidth;
                                        property.RoomQty = prop.RoomQty;
                                        property.RoofType = prop.RoofType;
                                        property.WallType = prop.WallType;
                                        property.SoilType = prop.SoilType;
                                        property.StructureUOM = prop.StructureUOM;
                                        property.MeasurementDate = prop.TopographDate;
                                        property.TrimbleCode = prop.TrimbleCode;
                                        property.TrimbleFile = prop.TrimbleFile;
                                        property.TopographerId = db.T_Employee.FirstOrDefault(e => e.EmployeeCode == prop.TopographerCode).EmployeeId;
                                        property.TopographerPoint = point.PointId;
                                        property.CultureStartDate = prop.CultureStartDate;
                                        property.MeasurementSignatureDate = prop.TopographDate;
                                        property.Comments = prop.Comments;
                                        property.Updated = DateTime.Now;
                                        property.UpdatedBy = loggedUser;
                                        db.Entry(property).State = EntityState.Modified;
                                    }

                                    if (!string.IsNullOrWhiteSpace(prop.CulturePoints))
                                    {
                                        string cultureP = prop.CulturePoints.Split('\\')[2];
                                        foreach (var file in attachments)
                                        {
                                            if (file.FileName == cultureP)
                                            {
                                                proServerPath = folderRoot+@"PAP\" + (prop.UserType == "U" ? ownerPapId.ToString() : pap.PAPId.ToString()) + @"\Properties\LAC" + pap.LACId.ToString() + @"\";
                                                if (file.FileName == proPictureName)
                                                {
                                                    if (!Directory.Exists(proServerPath))
                                                        Directory.CreateDirectory(proServerPath);

                                                    proServerPath = Path.Combine(proServerPath, Path.GetFileName(file.FileName));
                                                    file.SaveAs(proServerPath);
                                                }

                                                var attachment = new T_Attachment()
                                                {
                                                    RequestAttachementType = db.T_List.FirstOrDefault(l => l.ListName == "Attachment Type" && l.ListValue == "Property").ListId,
                                                    RequestAttachementPath = proServerPath,
                                                    RequestAttachementContentType = file.ContentType,
                                                    RequestAttachementFile = file.FileName,
                                                    Created = DateTime.Now,
                                                    CreatedBy = loggedUser,
                                                    Updated = DateTime.Now,
                                                    UpdatedBy = loggedUser
                                                };
                                                db.T_Attachment.Add(attachment);
                                                db.SaveChanges();

                                                db.T_PropertyAttachment.Add(new T_PropertyAttachment()
                                                {
                                                    PropertyId = property.PropertyId,
                                                    AttachmentId = attachment.AttachmentId,
                                                    Created = DateTime.Now,
                                                    CreatedBy = loggedUser,
                                                    Updated = DateTime.Now,
                                                    UpdatedBy = loggedUser
                                                });
                                                break;
                                            }
                                        }
                                    }

                                    newValue = JsonConvert.SerializeObject(property, settings);
                                    db.T_Log.Add(new T_Log()
                                    {
                                        Person_ID = loggedUser,
                                        Action = logAction,
                                        Table_Name = "T_Property",
                                        Table_Reference_ID = property.PropertyId,
                                        OldValue = oldValue,
                                        NewValue = newValue,
                                        LoggedDate = DateTime.Now
                                    });
                                    db.SaveChanges();
                                }
                            }

                            dbTransaction.Commit();
                            return success;
                        }
                        catch (Exception e)
                        {
                            List<string> exception = new List<string> { "Exception", e.Message };
                            try
                            {
                                dbTransaction.Rollback();
                            }
                            finally
                            {
                                //string serverPath = folderRoot+ @"HouseHold\LAC" + pap.LACId.ToString() + @"\" + hh.HouseHoldId.ToString() + @"\";
                                //if (Directory.Exists(serverPath))
                                //    Directory.Delete(serverPath, true);
                            }
                            return exception;
                        }
                    }
                }
            }
            else
            {
                error.Add("You don't have sufficient permissions to perform this task.");
                return error;
            }
        }
        #endregion
    }
}
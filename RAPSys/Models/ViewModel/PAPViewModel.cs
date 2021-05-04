using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace RapModel.ViewModel
{
    public class PAPViewModel
    {
        public int PersonId { get; set; }
        public int PAPId { get; set; }
        public string PersonType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string DateOfBirthString { get; set; }
        public string PlaceOfBirth { get; set; }
        public int? Father { get; set; }
        public string FatherName { get; set; }
        public int? Mother { get; set; }
        public string MotherName { get; set; }
        public int VulnerabilityTypeId { get; set; }
        public string VulnerabilityTypeName { get; set; }
        public string VulnerabilityDetail { get; set; }
        public int PAPLACId { get; set; }
        public int LACId { get; set; }
        public string LACName { get; set; }
        public string Comments { get; set; }
        public int? PaymentPreference { get; set; }
        public string PaymentPreferenceName { get; set; }
        public DateTime FormSubmissionDate { get; set; }
        public string FormSubmissionDateString { get; set; }
        public DateTime? PresurveyDate { get; set; }
        public string PresurveyDateString { get; set; }
        public int? Presurveyor { get; set; }
        public string PresurveyorCode { get; set; }
        public string PresurveyorName { get; set; }
        public string PresurveyorGPS { get; set; }
        public string PresurveyorCamera { get; set; }
        public DateTime? SurveyDate { get; set; }
        public string SurveyDateString { get; set; }
        public int? Surveyor { get; set; }
        public string SurveyorCode { get; set; }
        public string SurveyorName { get; set; }
        public bool SurveyedBefore { get; set; }
        public int? SurveyedFileID { get; set; }
        public int? SurveyedLacID { get; set; }
        public int? HouseHoldId { get; set; }
        public int PhotoID { get; set; }
        public string Picture { get; set; }
        public string PictureBase64 { get; set; }
        public string IdCard { get; set; }
        public int? Spouse { get; set; }
        public string SpouseName { get; set; }
        public string FileNumber { get; set; }
        public int? ResidenceID { get; set; }
        public string ResidenceName { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
        public HttpPostedFileBase[] PAPFile { get; set; }

        public PropertiesViewModel[] Properties { get; set; }
    }
}

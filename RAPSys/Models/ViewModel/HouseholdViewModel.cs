using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RapModel.ViewModel
{
    public class HouseholdViewModel
    {
        public int HouseHoldId { get; set; }
        public int HouseHoldResponsible { get; set; }
        public int HouseHoldNumber { get; set; }
        public bool PreviouslyCompensated { get; set; }
        public string CompensationUse { get; set; }
        public int ResidenceId { get; set; }
        public string ResidenceAddress { get; set; }
        public decimal? LastWeekExpense { get; set; }
        public bool FishOrMeat { get; set; }
        public bool EnoughFood { get; set; }
        public List<SocioEconomicElementsViewModel> SocioElements {get; set;}
        public int TabletsSource { get; set; }
        public string[] TabletsSourceName { get; set; }
        public int LastWeekDesease { get; set; }
        public string[] LastWeekDeseaseName { get; set; }
        public bool SkinDesease { get; set; }
        public int MosquitoNetSource { get; set; }
        public string[] MosquitoNetSourceName { get; set; }
        public int MosquitoNetUser { get; set; }
        public string[] MosquitoNetUserName { get; set; }
        public int SavingType { get; set; }
        public string[] SavingTypeName { get; set; }
        public string InterviewedName { get; set; }
        public int InterviewedRelationshipID { get; set; }
        public string InterviewedRelationship { get; set; }
        public int[] CultureTool { get; set; }
        public string[] CultureTools { get; set; }
        public bool NoRevenue { get; set; }
        public bool NoEquipment { get; set; }
        public bool NoAnimal { get; set; }
        public int PaidWorkersQty { get; set; }
        public string PaidWorkersTime { get; set; }

        public PAPViewModel PAPs { get; set; }
        public List<HouseholdMembersViewModel> HouseholdMembers { get; set; }
        public List<RevenueViewModel> HouseholdRevenues { get; set; }
        public List<ResidenceViewModel> HouseholdResidences { get; set; }
        public List<HouseholdCultureViewModel> HouseholdCultures { get; set; }
        public List<HouseHoldGoodViewModel> HouseholdGoods { get; set; }
        public List<HouseHoldPropertyViewModel> HouseholdProperties { get; set; }
        public List<HouseHoldPreviousCompensationViewModel> HouseholdCompensations { get; set; }
    }

    public class HouseholdMembersViewModel
    {
        public int PersonId { get; set; }
        public string PersonType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string IdCard { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string DateOfBirthString { get; set; }
        public string PlaceOfBirth { get; set; }
        public int? Father { get; set; }
        public string FatherName { get; set; }
        public int? Mother { get; set; }
        public string MotherName { get; set; }
        public string ParentPOB { get; set; }
        public string ParentPhone { get; set; }
        public int VulnerabilityTypeId { get; set; }
        public string VulnerabilityTypeName { get; set; }
        public string VulnerabilityDetail { get; set; }
        public int? HouseholdMemberId { get; set; }
        public int? HouseholdId { get; set; }
        public string MaritalStatus { get; set; }
        public int Relationship { get; set; }
        public string RelationshipName { get; set; }
        public bool IsStudent { get; set; }
        public int EducationLevel { get; set; }
        public string EducationLevelName { get; set; }
        public int FrenchLevel { get; set; }
        public string FrenchLevelName { get; set; }
        public string Picture { get; set; }
        public string PictureID { get; set; }
        public int ResidenceId { get; set; }
        public string ResidenceName { get; set; }
        public int Activity1 { get; set; }
        public string Activity1Name { get; set; }
        public int? Activity2 { get; set; }
        public string Activity2Name { get; set; }
        public int Competency1 { get; set; }
        public string Competency1Name { get; set; }
        public int? Competency2 { get; set; }
        public string Competency2Name { get; set; }
        public string Comments { get; set; }
    }

    public class RevenueViewModel
    {
        public int HouseHoldRevenueId { get; set; }
        public int HouseHoldId { get; set; }
        public int RevenueSource { get; set; }
        public string RevenueSourceName { get; set; }
        public decimal RevenueAmount { get; set; }
        public int MonthsPerYear { get; set; }
    }

    public class ResidenceViewModel
    {
        public int HouseholdResidenceId { get; set; }
        public int HouseholdId { get; set; }
        public int ResidenceId { get; set; }
        public bool IsOwner { get; set; }
        public int RoofType { get; set; }
        public string RoofTypeString { get; set; }
        public int WallType { get; set; }
        public string WallTypeString { get; set; }
        public int FloorType { get; set; }
        public string FloorTypeString { get; set; }
        public int RoomQty { get; set; }
        public decimal? Rent { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public string ArrivalDateString { get; set; }
        public int ArrivalReason { get; set; }
        public string ArrivalReasonName { get; set; }
        public int ToiletType { get; set; }
        public string ToiletTypeName { get; set; }
        public bool IsPermanent { get; set; }
        public string ResidenceAddress { get; set; }
    }

    public class HouseholdCultureViewModel
    {
        public int HouseholdCultureId { get; set; }
        public int HouseholdId { get; set; }
        public int AssetTypeId { get; set; }
        public string AssetType { get; set; }
        public decimal? Area { get; set; }
        public int AreaUOM { get; set; }
        public string AreaUOMString { get; set; }
        public int CultureDiversityId { get; set; }
        public string CultureDiversity { get; set; }
        public string CultureDescription { get; set; }
        public int PaidWorkersQty { get; set; }
        public string PaidWorkersTime { get; set; }
        public bool IsPurchased { get; set; }
        public bool IsSold { get; set; }
        public bool IsCultivated { get; set; }
        public int? CultureActionType { get; set; }
        public string CultureAction { get; set; }
        public decimal? HarvestLastSeason { get; set; }
        public int HarvestUOM { get; set; }
        public string HarvestUOMString { get; set; }
        public decimal? SoldLastSeason { get; set; }
        public int SoldUOM { get; set; }
        public string SoldUOMString { get; set; }
        public decimal? Revenue { get; set; }
        public int RevenueUOM { get; set; }
        public string RevenueUOMString { get; set; }
        public string Comments { get; set; }
    }

    public class HouseHoldGoodViewModel
    {
        public int HouseHoldGoodId { get; set; }
        public int HouseholdId { get; set; }
        public int Good { get; set; }
        public string GoodName { get; set; }
        public string GoodDescription { get; set; }
        public int Quantity { get; set; }
        public int QuantityUOM { get; set; }
        public string QuantityUOMString { get; set; }
        public string GoodType { get; set; }
    }

    public class HouseHoldPropertyViewModel
    {
        public int HouseHoldPropertyId { get; set; }
        public int HouseholdId { get; set; }
        public int LandId { get; set; }
        public string LandName { get; set; }
        public int RegionId { get; set; }
        public string RegionName { get; set; }
        public int DistanceFromResidence { get; set; }
        public int DistanceUOM { get; set; }
        public string DistanceUOMString { get; set; }
        public int[] PropertyWorkerType { get; set; }
        public string[] PropertyWorkerTypeString { get; set; }
        public decimal? CultivatedArea { get; set; }
        public int CultivatedAreaUOM { get; set; }
        public string CultivatedAreaUOMString { get; set; }
        public decimal? FallowArea { get; set; }
        public int FallowAreaUOM { get; set; }
        public string FallowAreaUOMString { get; set; }
        public bool IsOwner { get; set; }
        public decimal? Rent { get; set; }
        public int PropertySource { get; set; }
        public string PropertySourceName { get; set; }
        public int? AcquisitionYear { get; set; }
        public bool IsCultivatedLastYear { get; set; }
    }

    public class HouseHoldPreviousCompensationViewModel{
        public int Year { get; set; }
        public decimal Amount { get; set; }
        public int Surface { get; set; }
        public int SurfaceUOM { get; set; }
        public string SurfaceUOMString { get; set; }
        }
}
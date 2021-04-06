using System;

namespace RapModel.ViewModel
{
    public class StructureViewModel
    {
        public int StructureID { get; set; }
        public string StructureCode { get; set; }
        public string StructureLabel { get; set; }
        public string StructureUsageCode { get; set; }
        public string StructureUsageLabel { get; set; }
        public string StructureOwnerCode { get; set; }
        public string StructureOwnerLabel { get; set; }
    }

    public class MaterialsViewModel
    {
        public int MaterialsID { get; set; }
        public string MaterialsCode { get; set; }
        public string MaterialsLabel { get; set; }
    }

    public class TreeViewModel
    {
        public int TreeID { get; set; }
        public string TreeName { get; set; }
        public int TreeMaturity { get; set; }
        public string TreeMaturityName { get; set; }
        public string TreeMaturityLabel { get; set; }
        public int TreeCount { get; set; }
        public string GPSID { get; set; }
    }

    public class PaymentPreferenceViewModel
    {
        public int PreferenceId { get; set; }
        public string PreferenceName { get; set; }
        public string PreferenceLabel { get; set; }
    }

    public class VulnerabilityViewModel
    {
        public int VulnerabilityId { get; set; }
        public string VulnerabilityName { get; set; }
        public string VulnerabilityLabel { get; set; }
    }

    public class ListViewModel
    {
        public int ListId { get; set; }
        public string ListName { get; set; }
        public string ListValue { get; set; }
        public string ListLabel { get; set; }
    }

    public class CultureToolViewModel
    {
        public int CultureToolId { get; set; }
        public string CultureToolType { get; set; }
    }

    public class CultureDiversityViewModel
    {
        public int CultureDiversityId { get; set; }
        public string CultureDiversityName { get; set; }
    }

    public class SocioEconomicElementsViewModel
    {
        public int HHSocioElementsId { get; set; }
        public int SocioElementsId { get; set; }
        public string SocioElementName { get; set; }
        public string SocioElementValue { get; set; }
        public int HouseHoldId { get; set; }
    }
}

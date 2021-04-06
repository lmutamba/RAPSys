using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RapModel.ViewModel
{
    public class LACViewModel
    {
        public int LACId { get; set; }
        public string LAC_ID { get; set; }
        public string LACName { get; set; }
        public int LACRequestId { get; set; }
        public int LACStatus { get; set; }
        public string LacStatusString { get; set; }
        public decimal Realcosts { get; set; }
        public int PAPs { get; set; }
        public decimal? AuthorizedAreaSize { get; set; }
        public string AuthorizedAreaSizeString { get; set; }
        public int AuthorizedAreaSizeUOM { get; set; }
        public string AuthorizedAreaSizeUOMDescription { get; set; }
        public DateTime AuthorizedDate { get; set; }
        public string AuthorizedDateString { get; set; }
        public string AreaDescription { get; set; }
        public decimal? AreaRequested { get; set; }
        public int? AreaRequestedUOM { get; set; }
        public string AreaRequestedUOMString { get; set; }
        public string AreaRequestedSize { get; set; }
        public decimal AreaCompensed { get; set; }
        public int AreaCompensedUOM { get; set; }
        public string AreaCompensedUOMString { get; set; }
        public string AreaCompensedString { get; set; }
        public int? LandID { get; set; }
        public int LandLacID { get; set; }
        public decimal? CostEstimate { get; set; }
        public string Comment { get; set; }
        public DateTime RequestDate { get; set; }
        public string RequestDateString { get; set; }
        public string RegionName { get; set; }
        public string Department { get; set; }
        public bool Locked { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
        public LandRequestViewModel LandRequest { get; set; }
        public PAPViewModel[] PapLac { get; set; }
        public AttachmentViewModel[] AttachmentsDocuments { get; set; }
    }
}

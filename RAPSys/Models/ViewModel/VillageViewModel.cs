using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RapModel.ViewModel
{
    public class VillageViewModel
    {
        public int LacRequestId { get; set; }
        public int LandId { get; set; }
        public int VillageId { get; set; }
        public string VillageName { get; set; }
        public string VillageOldName { get; set; }
        public int VillageStatus { get; set; }
        public string VillageStatusString { get; set; }
        public int RegionId { get; set; }
        public string RegionName { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
    }
}

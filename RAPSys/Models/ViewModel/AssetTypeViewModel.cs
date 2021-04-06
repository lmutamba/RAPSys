using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RapModel.ViewModel
{
    public class AssetTypeViewModel
    {
        public int AssetTypeId { get; set; }
        public string AssetName { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
    }
}
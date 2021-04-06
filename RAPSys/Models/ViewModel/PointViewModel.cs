using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RapModel.ViewModel
{
    public class PointViewModel
    {
        public int PointId { get; set; }
        public string PointName { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal? Elevation { get; set; }
        public int LandId { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
    }
}
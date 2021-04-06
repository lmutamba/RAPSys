using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RapModel.ViewModel
{
    public class UOMViewModel
    {
        public int UOMId { get; set; }
        public string UOM { get; set; }
        public int? UOMReference { get; set; }
        public decimal UOMConversion { get; set; }
        public int UOMType { get; set; }
        public string UOMTypeString { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
    }
}

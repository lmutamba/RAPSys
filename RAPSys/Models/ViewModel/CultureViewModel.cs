using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RapModel.ViewModel
{
    public class CultureViewModel
    {
        public int CultureId { get; set; }
        public string CultureName { get; set; }
        public string CultureType { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
    }
}

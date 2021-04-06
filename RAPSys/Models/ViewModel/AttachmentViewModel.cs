using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RapModel.ViewModel
{
    public class AttachmentViewModel
    {
        public int AttachmentId { get; set; }
        public int RequestAttachementType { get; set; }
        public string AttachmentType { get; set; }
        public string RequestAttachementPath { get; set; }
        public string RequestAttachementFile { get; set; }
        public string RequestAttachementContentType { get; set; }
        public int RequestId { get; set; }
        public int LandId { get; set; }
        public int PropertyId { get; set; }
        public int PAPLACId { get; set; }
        public int LacId { get; set; }
    }
}
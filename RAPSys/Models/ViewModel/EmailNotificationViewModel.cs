using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RapModel.ViewModel
{
    public class EmailNotificationViewModel
    {
        public List<string> To { get; set; } = new List<string>();
        public List<string> Cc { get; set; } = new List<string>();
        public List<string> Bcc { get; set; } = new List<string>();
        public string FromEmail { get; set; }
        public string FromName { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public System.Net.Mail.Attachment [] Attachment{get; set;}

    }
}
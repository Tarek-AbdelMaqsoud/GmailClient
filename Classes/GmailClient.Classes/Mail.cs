using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmailClient.Classes
{
    public class Mail
    {
        public int SerialNo { get; set; }
        public string UniqueId { get; set; }
        public string FromDisplayName { get; set; }
        public string FromEmail { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public System.DateTime TimeReceived { get; set; }
        public bool HasAttachment { get; set; }
        public string Body { get; set; }
        public string ToAsCsv { get; set; }
        public long MessageId { get; set; }
    }
}

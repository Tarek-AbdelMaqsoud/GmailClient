using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace GmailClient.Constants
{
    public static class Constants
    {
        public static string clientID = (string)WebConfigurationManager.AppSettings["ClientID"];
        public static string clientSecret = (string)WebConfigurationManager.AppSettings["ClientSecret"];
        public static string redirectUri = (string)WebConfigurationManager.AppSettings["RedirectURI"];
        public static string authorizationEndpoint = (string)WebConfigurationManager.AppSettings["AuthorizationEndpoint"];
        public static string tokenEndpoint = (string)WebConfigurationManager.AppSettings["TokenEndpoint"];

        public static string ImapAndSmtpScope = (string)WebConfigurationManager.AppSettings["ImapAndSmtpScope"];
        public static string EmailScope = (string)WebConfigurationManager.AppSettings["EmailScope"];
        public static string SMTPServer = (string)WebConfigurationManager.AppSettings["SMTPServer"];
        public static string SMTPPort= (string)WebConfigurationManager.AppSettings["SMTPPort"];
        public static string ImapHost = (string)WebConfigurationManager.AppSettings["ImapHost"];
        public static string ImapPort = (string)WebConfigurationManager.AppSettings["ImapPort"];
        public static string MessagesPerPage = (string)WebConfigurationManager.AppSettings["MessagesPerPage"];



    }
}

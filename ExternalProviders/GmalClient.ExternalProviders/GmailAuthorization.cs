using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetOpenAuth.OAuth2;
using DotNetOpenAuth.Messaging;

namespace GmailClient.ExternalProviders
{
    /// <summary>
    /// GmailAuthorization is to provide the external Authentication and Authorization by Google.
    /// Here, we have created an App on Google to grant an access to the Gmail API
    /// This App will ask each user trying to connect to Gmail through it for his permission
    /// </summary>
    public class GmailAuthorization
    {
        public void AuthorizeWithGoogle()
        {
            try
            {
                AuthorizationServerDescription server = new AuthorizationServerDescription
                {
                    AuthorizationEndpoint = new Uri(GmailClient.Constants.Constants.authorizationEndpoint),
                    TokenEndpoint = new Uri(GmailClient.Constants.Constants.tokenEndpoint),
                    ProtocolVersion = ProtocolVersion.V20,
                };

                List<string> scope = new List<string>
                {
                    GmailClient.Constants.Constants.ImapAndSmtpScope,
                    GmailClient.Constants.Constants.EmailScope
                };

                //Getting the client using our ClientID and Client Secret provided by Google to be used as identification for our App
                WebServerClient consumer = new WebServerClient(server, GmailClient.Constants.Constants.clientID, GmailClient.Constants.Constants.clientSecret);

                // Here redirect to authorization site (Gmail) occurs
                consumer.RequestUserAuthorization(scope, new Uri(GmailClient.Constants.Constants.redirectUri));
                OutgoingWebResponse response = consumer.PrepareRequestUserAuthorization(scope, new Uri(GmailClient.Constants.Constants.redirectUri));
            }
            catch (Exception ex)
            {

            }
        }
        public void AuthorizeUser()
        {
            try
            {
                AuthorizationServerDescription server = new AuthorizationServerDescription
                {
                    AuthorizationEndpoint = new Uri(GmailClient.Constants.Constants.authorizationEndpoint),
                    TokenEndpoint = new Uri(GmailClient.Constants.Constants.tokenEndpoint),
                    ProtocolVersion = ProtocolVersion.V20,
                };

                List<string> scope = new List<string>
                {
                    GmailClient.Constants.Constants.ImapAndSmtpScope,
                    GmailClient.Constants.Constants.EmailScope
                };

                //Getting the client using our ClientID and Client Secret provided by Google to be used as identification for our App
                WebServerClient consumer = new WebServerClient(server, GmailClient.Constants.Constants.clientID, GmailClient.Constants.Constants.clientSecret);

                // Here redirect to authorization site (Gmail) occurs
                consumer.RequestUserAuthorization(scope, new Uri(GmailClient.Constants.Constants.redirectUri));
                OutgoingWebResponse response = consumer.PrepareRequestUserAuthorization(scope, new Uri(GmailClient.Constants.Constants.redirectUri));
            }
            catch (Exception ex)
            {

            }
        }

    }
}

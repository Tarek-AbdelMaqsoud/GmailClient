using DotNetOpenAuth.OAuth2;
using Limilabs.Client.Authentication.Google;
using Limilabs.Client.IMAP;
using Limilabs.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using DotNetOpenAuth.Messaging;

namespace GmailClient.ExternalProviders
{
    public class GmailPaidLibrary : GmailLibrary
    {
        public List<Classes.Mail> GetMails(int nextPageNumber, out string accessToken, out int messagesCount)
        {
            //Start and End Serial Numbers for paging
            int startSerialNumber = ((nextPageNumber - 1) * Convert.ToInt32(Constants.Constants.MessagesPerPage)) + 1;
            int endSerialNumber = ((nextPageNumber) * Convert.ToInt32(Constants.Constants.MessagesPerPage));
            var mails = new List<Classes.Mail>();
            accessToken = string.Empty;

            AuthorizationServerDescription server = new AuthorizationServerDescription
            {
                AuthorizationEndpoint = new Uri(GmailClient.Constants.Constants.authorizationEndpoint),
                TokenEndpoint = new Uri(GmailClient.Constants.Constants.tokenEndpoint),
                ProtocolVersion = ProtocolVersion.V20,
            };


            WebServerClient consumer = new WebServerClient(server, GmailClient.Constants.Constants.clientID, GmailClient.Constants.Constants.clientSecret);
            consumer.ClientCredentialApplicator =
                ClientCredentialApplicator.PostParameter(GmailClient.Constants.Constants.clientSecret);
            IAuthorizationState grantedAccess = consumer.ProcessUserAuthorization(null);

            //Getting the access token and save it to be used later
            accessToken = grantedAccess.AccessToken;

            //This section can be enhanced in case of cached
            GoogleApi api = new GoogleApi(accessToken);
            string user = api.GetEmailPlus();
            using (Imap imap = new Imap())
            {
                //Connecting to the Imap
                imap.ConnectSSL(Constants.Constants.ImapHost);

                //Authenticating using the Access Token
                imap.LoginOAUTH2(user, accessToken);
                imap.SelectInbox();
                List<long> uids = imap.GetAll();
                messagesCount = uids.Count;
                uids = uids.OrderByDescending(x=>x).ToList();
                //Apply paging
                uids = uids.Take(Convert.ToInt32(Constants.Constants.MessagesPerPage)).Skip(startSerialNumber).ToList();
                foreach (long uid in uids)
                {
                    var eml = imap.GetMessageByUID(uid);
                    IMail email = new MailBuilder().CreateFromEml(eml);
                    Classes.Mail tempEmail = new Classes.Mail()
                    {
                        SerialNo = startSerialNumber,
                        UniqueId = uid.ToString(),
                        FromDisplayName = email.From.First().Name,
                        FromEmail = email.From.First().ToString(),
                        To = email.To.ToString(),
                        Subject = email.Subject,
                        TimeReceived = email.Date.Value,
                        HasAttachment = email.Attachments.Count() > 0 ? true : false,
                        Body = email.Text,
                    };
                    mails.Add(tempEmail);
                }
                imap.Close();
            }
            return mails;
        }

        public Classes.Mail GetMail(int MessageId, string accessToken)
        {
            AuthorizationServerDescription server = new AuthorizationServerDescription
            {
                AuthorizationEndpoint = new Uri(GmailClient.Constants.Constants.authorizationEndpoint),
                TokenEndpoint = new Uri(GmailClient.Constants.Constants.tokenEndpoint),
                ProtocolVersion = ProtocolVersion.V20,
            };

            //Authenticate using mail and password
            WebServerClient consumer = new WebServerClient(server, GmailClient.Constants.Constants.clientID, GmailClient.Constants.Constants.clientSecret);
            consumer.ClientCredentialApplicator =
                ClientCredentialApplicator.PostParameter(GmailClient.Constants.Constants.clientSecret);

            //Get the access Token
            IAuthorizationState grantedAccess = consumer.ProcessUserAuthorization(null);
            if (string.IsNullOrEmpty(accessToken))
            {
                accessToken = grantedAccess.AccessToken;
            }

            //Authenticate with the access Token 
            GoogleApi api = new GoogleApi(accessToken);
            string user = api.GetEmailPlus();
            using (Imap imap = new Imap())
            {
                imap.ConnectSSL(Constants.Constants.ImapHost);
                imap.LoginOAUTH2(user, accessToken);
                imap.SelectInbox();
                var eml = imap.GetMessageByUID(MessageId);
                IMail email = new MailBuilder().CreateFromEml(eml);
                Classes.Mail tempEmail = new Classes.Mail()
                {
                    UniqueId = MessageId.ToString(),
                    FromDisplayName = email.From.First().Name,
                    FromEmail = email.From.First().ToString(),
                    To = email.To.ToString(),
                    Subject = email.Subject,
                    TimeReceived = email.Date.Value,
                    HasAttachment = email.Attachments.Count() > 0 ? true : false,
                    Body = email.Text,
                };
                imap.Close();
                return tempEmail;
            }
        }

        public string Delete(Model.UsersDb user, string messagesIds)
        {
            using (Imap imap = new Imap())
            {
                imap.ConnectSSL(Constants.Constants.ImapHost);
                imap.UseBestLogin(user.GmailConfig.GmailId, Helpers.Password.Decrypt(user.GmailConfig.GmailPassword));
                // Recognize Trash folder
                List<FolderInfo> folders = imap.GetFolders();
                CommonFolders common = new CommonFolders(folders);
                FolderInfo trash = common.Trash;

                // Find all emails we want to delete
                imap.SelectInbox();
                List<long> uids = new List<long>();
                if (messagesIds.Contains(','))
                {
                    foreach (var item in messagesIds.Split(','))
                    {
                        uids.Add(Convert.ToInt64(item));
                    }
                }
                else
                {
                    uids.Add(Convert.ToInt64(messagesIds));
                }
                // Move email to Trash
                List<long> uidsInTrash = new List<long>();
                foreach (long uid in uids)
                {
                    long uidInTrash = (long)imap.MoveByUID(uid, trash);
                    uidsInTrash.Add(uidInTrash);
                }
                // Delete moved emails from Trash
                imap.Select(trash);
                foreach (long uid in uidsInTrash)
                {
                    imap.DeleteMessageByUID(uid);
                }
                imap.Close();
            }
            return string.Empty;

        }

    }
}

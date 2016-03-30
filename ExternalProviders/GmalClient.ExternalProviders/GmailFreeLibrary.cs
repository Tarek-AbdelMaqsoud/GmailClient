using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GmailClient.Classes;
using MailKit.Net.Imap;
using System.Net;
using MailKit;

namespace GmailClient.ExternalProviders
{
    public class GmailFreeLibrary : GmailLibrary
    {
        public List<Classes.Mail> GetMails(Model.UsersDb user, int nextPageNumber, out int messagesCount)
        {
            //Start and end Serial Number For Paging
            int startSerialNumber = ((nextPageNumber - 1) * Convert.ToInt32(Constants.Constants.MessagesPerPage)) + 1;
            int endSerialNumber = ((nextPageNumber) * Convert.ToInt32(Constants.Constants.MessagesPerPage));
            var emails = new List<Classes.Mail>();

            using (ImapClient client = new ImapClient())
            {
                client.Connect(Constants.Constants.ImapHost, Convert.ToInt32(Constants.Constants.ImapPort), true);
                var gmaiConfigRep = new Repository.GmailConfigRepository();

                client.Authenticate(new NetworkCredential(user.GmailConfig.GmailId, Helpers.Password.Decrypt(user.GmailConfig.GmailPassword)));

                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadOnly);
                messagesCount = inbox.Count;
                if (inbox.Count > 0)
                {
                    int pageEndIndex = Math.Max(inbox.Count - startSerialNumber, 0);
                    int pageStartIndex = Math.Max(inbox.Count - endSerialNumber, 0);

                    var messages = inbox.Fetch(pageStartIndex, pageEndIndex, MessageSummaryItems.Envelope);

                    messages = messages.OrderByDescending(message => message.Envelope.Date.Value.DateTime).ToList();
                    foreach (var message in messages)
                    {
                        if (startSerialNumber <= endSerialNumber)
                        {
                            Classes.Mail tempEmail = new Classes.Mail()
                            {
                                SerialNo = startSerialNumber,
                                UniqueId = message.Index.ToString(),
                                FromDisplayName = message.Envelope.From.First().Name,
                                FromEmail = message.Envelope.From.First().ToString(),
                                To = message.Envelope.To.ToString(),
                                Subject = message.NormalizedSubject,
                                TimeReceived = message.Envelope.Date.Value.DateTime,
                                HasAttachment = message.Attachments.Count() > 0 ? true : false,
                                Body = inbox.GetMessage(message.Index).TextBody
                            };
                            emails.Add(tempEmail);
                            startSerialNumber++;
                        }
                    }
                }
            }
            return emails;
        }

        public Classes.Mail GetMail(Model.UsersDb user, int MessageId)
        {
            using (ImapClient client = new ImapClient())
            {
                client.Connect(Constants.Constants.ImapHost, Convert.ToInt32(Constants.Constants.ImapPort), true);

                client.Authenticate(new NetworkCredential(user.GmailConfig.GmailId, Helpers.Password.Decrypt(user.GmailConfig.GmailPassword)));

                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadOnly);

                if (inbox.Count > 0)
                {
                    var message = inbox.GetMessage(MessageId);

                    return new Classes.Mail()
                    {
                        UniqueId = MessageId.ToString(),
                        FromDisplayName = message.From.First().Name,
                        FromEmail = message.From.First().ToString(),
                        To = message.To.ToString(),
                        Subject = message.Subject,
                        TimeReceived = message.Date.DateTime,
                        HasAttachment = message.Attachments.Count() > 0 ? true : false,
                        Body = message.TextBody
                    };
                }
            }
            return null;
        }

        public string Delete(Model.UsersDb user, string messagesIds)
        {
            var outputMessage = "";
            try
            {


                using (ImapClient client = new ImapClient())
                {
                    client.Connect(Constants.Constants.ImapHost, Convert.ToInt32(Constants.Constants.ImapPort), true);
                    client.Authenticate(new NetworkCredential(user.GmailConfig.GmailId, Helpers.Password.Decrypt(user.GmailConfig.GmailPassword)));

                    var inbox = client.Inbox;
                    inbox.Open(FolderAccess.ReadWrite);

                    var uids = new List<UniqueId>();

                    if (messagesIds.Contains(','))
                    {
                        foreach (var item in messagesIds.Split(','))
                        {
                            uids.Add(new UniqueId(Convert.ToUInt32(item)));
                        }
                    }
                    else
                    {
                        uids.Add(new UniqueId(Convert.ToUInt32(messagesIds)));
                    }

                    client.Inbox.AddFlags(uids, MailKit.MessageFlags.Deleted, true);

                    if (client.Capabilities.HasFlag(ImapCapabilities.UidPlus))
                    {
                        client.Inbox.Expunge(uids);
                    }
                    else
                    {
                        client.Inbox.Expunge();
                    }
                    outputMessage = "Email(s) deleted successfully!";
                }
            }
            catch (Exception ex)
            {
                outputMessage = "There was an error in processing your request. Exception: " + ex.Message;
            }
            return outputMessage;
        }
    }
}

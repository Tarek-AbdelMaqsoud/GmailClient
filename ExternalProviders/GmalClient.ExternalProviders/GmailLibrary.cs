using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GmailClient.ExternalProviders
{
    public abstract class GmailLibrary
    {
        public string Compose(Model.UsersDb user, Classes.Mail email)
        {
            string outputMessage = "";
            try
            {

                email.FromEmail = user.GmailConfig.GmailId;

                var message = new MimeMessage();

                message.From.Add(new MailboxAddress(email.FromEmail, email.FromEmail));

                if (email.ToAsCsv.Contains(','))
                {
                    foreach (var item in email.ToAsCsv.Split(','))
                    {
                        message.To.Add(new MailboxAddress(item, item));
                    }
                }
                else if (email.ToAsCsv.Contains(';'))
                {
                    foreach (var item in email.ToAsCsv.Split(';'))
                    {
                        message.To.Add(new MailboxAddress(item, item));
                    }
                }
                else
                {
                    message.To.Add(new MailboxAddress(email.ToAsCsv, email.ToAsCsv));
                }
                message.Subject = email.Subject;
                message.Body = new TextPart("plain")
                {
                    Text = email.Body
                };

                using (var client = new SmtpClient())
                {
                    try
                    {
                        client.Connect(Constants.Constants.SMTPServer,Convert.ToInt32(Constants.Constants.SMTPPort));

                        client.Authenticate(new NetworkCredential(user.GmailConfig.GmailId, Helpers.Password.Decrypt(user.GmailConfig.GmailPassword)));

                        client.Send(message);
                        client.Disconnect(true);

                        outputMessage = "Your message was sent successfully";
                    }
                    catch (Exception ex)
                    {
                        outputMessage = "There was an error sending your mail.";
                    }
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

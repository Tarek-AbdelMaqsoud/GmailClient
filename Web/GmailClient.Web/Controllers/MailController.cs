using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GmailClient.Web.Models;
using System.Web.Configuration;
using System.Net;

namespace GmailClient.Web.Controllers
{
    public class MailController : Controller
    {
        //
        // GET: /Mail/
        [HttpGet]
        [Authorize]
        public ActionResult Index()
        {
            var user = Session["LoggedInUser"] as Model.UsersDb;
            if (user != null && user.GmailConfigId != null)
            {
                return RedirectToAction("Identify", "Mail");
            }
            else
            {
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(RegisterGmailViewModel model)
        {
            Repository.GmailConfigRepository gmailRep = new Repository.GmailConfigRepository();
            var gmail = new Model.GmailConfigDb();
            gmail.GmailId = model.GmailID;
            gmail.GmailPassword = Helpers.Password.Encrypt(model.GmailPassword);
            gmail = gmailRep.Add(gmail);

            var user = Session["LoggedInUser"] as Model.UsersDb;
            user.GmailConfigId = gmail.GmailConfigId;

            Repository.UserRepository userRep = new Repository.UserRepository();
            userRep.Modify(user);

            user.GmailConfig = gmail;
            Session["LoggedInUser"] = user;

            return RedirectToAction("Identify", "Mail");
        }

        [HttpGet]
        [Authorize]
        public ActionResult Message(int messageId)
        {
            var isFree = Session["isFree"] as bool?;
            var user = Session["LoggedInUser"] as Model.UsersDb;
            var message = new GmailClient.Classes.Mail();
            var gmailPaidLibrary = new GmailClient.ExternalProviders.GmailPaidLibrary();
            var gmailFreeLibrary = new GmailClient.ExternalProviders.GmailFreeLibrary();

            if (isFree != null)
            {
                if (isFree.Value)
                {
                    message = gmailFreeLibrary.GetMail(user, messageId);
                }
                else
                {
                    string accessToken = Session["AccessToken"] as string;
                    message = gmailPaidLibrary.GetMail(messageId, accessToken);
                }

            }
            return View(message);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Message(RegisterGmailViewModel model)
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult Identify()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Identify(FormCollection form)
        {
            bool isFree = false;
            if (form["isFree"] != null)
            {
                isFree = bool.Parse(form["isFree"]);
            }

            return RedirectToAction("List", "Mail", new { isFree = isFree });
        }
        [HttpPost]
        public JsonResult Compose(Classes.Mail email)
        {
            JsonResult jsonResult = new JsonResult();
            var user = Session["LoggedInUser"] as Model.UsersDb;
            var gmailFreeLbrary = new GmailClient.ExternalProviders.GmailFreeLibrary();

            jsonResult.Data = new
            {
                message = gmailFreeLbrary.Compose(user, email),
            };
            return jsonResult;
        }

        [HttpPost]
        public JsonResult Delete(string csEmailUids)
        {
            JsonResult jsonResult = new JsonResult();
            var isFree = Session["isFree"] as bool?;
            var user = Session["LoggedInUser"] as Model.UsersDb;

            if (isFree.HasValue && isFree.Value)
            {
                var gmailFreeLibrary = new GmailClient.ExternalProviders.GmailFreeLibrary();
                jsonResult.Data = new
                {
                    message = gmailFreeLibrary.Delete(user, csEmailUids),
                };
            }
            else
            {
                var gmailPaidLibrary = new GmailClient.ExternalProviders.GmailPaidLibrary();
                jsonResult.Data = new
                {
                    message = gmailPaidLibrary.Delete(user, csEmailUids),
                };
            }
            return jsonResult;
        }

        [HttpGet]
        [Authorize]
        public ActionResult List(bool isFree)
        {
            var user = Session["LoggedInUser"] as Model.UsersDb;

            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            else if (user != null && user.GmailConfigId == null)
            {
                return RedirectToAction("Index", "Mail");
            }
            Session["isFree"] = isFree;

            var gmailAuthorization = new GmailClient.ExternalProviders.GmailAuthorization();
            gmailAuthorization.AuthorizeWithGoogle();

            //Return null as the authorization redirection will be done
            return null;
        }

        [HttpGet]
        [Authorize]
        public ActionResult ListAll(int? pageNumber)
        {
            var isFree = Session["isFree"] as bool?;
            var user = Session["LoggedInUser"] as Model.UsersDb;
            var emails = new List<Classes.Mail>();
            var gmailPaidLibrary = new GmailClient.ExternalProviders.GmailPaidLibrary();
            var gmailFreeLibrary = new GmailClient.ExternalProviders.GmailFreeLibrary();
            pageNumber = pageNumber ?? 1;
            ViewBag.PageNumber = pageNumber;
            int messagesCount = 0;
            if (isFree.HasValue && isFree.Value)
            {
                emails = gmailFreeLibrary.GetMails(user, pageNumber.Value, out messagesCount);
            }
            else
            {
                var accessToken = string.Empty;
                emails = gmailPaidLibrary.GetMails(pageNumber.Value, out accessToken, out messagesCount);
                Session["AccessToken"] = accessToken;
            }
            ViewBag.MessagesCount = messagesCount;
            return View(emails);
        }
      
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using GmailClient.Web.Models;
using System.Web.Helpers;

namespace GmailClient.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public AccountController()
        {
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                Repository.UserRepository usersRep = new Repository.UserRepository();

                var encryptedPassword = Helpers.Password.Encrypt(model.Password);
                var user = usersRep.Get(model.UserName, encryptedPassword);
                if (user != null)
                {
                    login(user);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                Repository.UserRepository usersRep = new Repository.UserRepository();
                var user = new Model.UsersDb();
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Username = model.UserName;
                user.Password = Helpers.Password.Encrypt(model.Password);

                user = usersRep.Add(user);
                login(user);
                return RedirectToAction("Index", "Home");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            Session["LoggedInUser"] = null;
            HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        #region Helper
        private void login(Model.UsersDb user)
        {

            Session["LoggedInUser"] = user;
            HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            Claim loginClaim = new Claim(ClaimTypes.Name, user.Username);
            Claim[] claims = new Claim[] { loginClaim, new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", user.Username), new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", user.Username) };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

            HttpContext.GetOwinContext().Authentication.SignIn(new AuthenticationProperties() { IsPersistent = false }, claimsIdentity);
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
        }
        #endregion
    }
}
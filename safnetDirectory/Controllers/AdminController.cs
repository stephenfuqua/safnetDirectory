using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using safnetDirectory.FullMvc.Models;

namespace safnetDirectory.FullMvc.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        public const string HR_ROLE = "HR User";

        private IApplicationUserManager _userManager;

        public AdminController()
        {

        }

        public AdminController(IApplicationUserManager userManager, IApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public IApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>() as IApplicationUserManager;
            }
            private set
            {
                _userManager = value;
            }
        }

      

        private IApplicationSignInManager _signInManager;

        public IApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>() as IApplicationSignInManager;
            }
            private set { _signInManager = value; }
        }

      

        //
        // GET: /Account/Register
        [Authorize(Roles=HR_ROLE)]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [Authorize(Roles = HR_ROLE)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid && model != null)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    PhoneNumber = model.OfficePhoneNumber,
                    MobilePhoneNumber = model.MobilePhoneNumber,
                    Title = model.Title,
                    Location = model.Location
                };
                var result = await UserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    if (model.IsHrUser)
                    {
                        // TODO: handle error when the role doesn't exist

                        result = await UserManager.AddToRoleAsync(user.Id, HR_ROLE);

                        if (!result.Succeeded)
                        {
                            AddErrors(result);
                        }
                    }
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

    }
}
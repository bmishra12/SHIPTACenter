using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using umbraco.cms.businesslogic.member;
using System.ComponentModel.DataAnnotations;
using UmbracoShipTac.Models;
using Umbraco.Web.Mvc;

namespace UmbracoShipTac.Controllers
{


    public class MemberLoginFormSurfaceController : SurfaceController
    {
        [HttpGet]
        [ActionName("MemberLogin")]
        public ActionResult MemberLoginGet()
        {
          MemberLoginFormViewModel model =  new MemberLoginFormViewModel();
            model.ReturnUrl = Request.Url.AbsoluteUri;
            return PartialView("MemberLoginForm", model);
        }

        [HttpGet]
        public ActionResult MemberLogout()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return Redirect("/");
        }

        [HttpPost]
        [ActionName("MemberLogin")]
        public ActionResult MemberLoginPost(MemberLoginFormViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.Username, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                   
                    else
                    {
                        
                        return RedirectToCurrentUmbracoPage();
                    }
                }
                else
                {
                    TempData["Status"] = "Invalid username or password";
                    return RedirectToCurrentUmbracoPage();
                }
            }

            TempData["Status"] = "Invalid request";
            return RedirectToCurrentUmbracoPage();
        }
    }
}
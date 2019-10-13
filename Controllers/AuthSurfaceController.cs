using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Umbraco.Web.Mvc;
using UmbracoShipTac.Models;
using UmbracoShipTac.Code;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.member;
using Umbraco.Core;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;



namespace UmbracoShipTac.Controllers.SurfaceControllers
{



    public class AuthSurfaceController : SurfaceController
    {

        /// <summary>
        /// Renders the Login view
        /// @Html.Action("RenderLogin","AuthSurface");
        /// </summary>
        /// <returns></returns>
        ///         

        //sammit: we are not using this...
        public ActionResult RenderLoginOLD()
        {
            LoginViewModel loginModel = new LoginViewModel();


            if (string.IsNullOrEmpty(HttpContext.Request["ReturnUrl"]))
            {
                //If returnURL is empty then set it to /
                loginModel.ReturnUrl = "/";
            }
            else
            {
                //Lets use the return URL in the querystring or form post
                loginModel.ReturnUrl = HttpContext.Request["ReturnUrl"];
            }

            return PartialView("Login", loginModel);
        }


        /// <summary>
        /// /
        /// </summary>
        /// <returns></returns>
        /// 
        [ChildActionOnly]

        public ActionResult RenderLogin()
        {
            LoginViewModel model = new LoginViewModel();

            model.ReturnUrl = Request.Url.AbsoluteUri;


            //if (string.IsNullOrEmpty(HttpContext.Request["ReturnUrl"]))
            //{
            //    //If returnURL is empty then set it to /
            //    model.ReturnUrl = "/";
            //}
            //else
            //{
            //    //Lets use the return URL in the querystring or form post
            //    model.ReturnUrl = HttpContext.Request["ReturnUrl"];
            //}
            return PartialView("authsurface/login", model);
        }





        /// <summary>
        /// Handles the login form when user posts the form/attempts to login
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// to avoid The provided anti-forgery token was meant for user "", but the current user is "myUsername". error
        /// added  [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult HandleLogin(LoginViewModel model)
        {

            if (!ModelState.IsValid)
            {

                return CurrentUmbracoPage();
            }

            //Member already logged in - redirect to feature page
            if (IsLoggedOn())
            {
                return Redirect("/feature");
            }


            //remove the extra space if there 
            model.EmailAddress = model.EmailAddress.Trim();


            //Get the member from their email address
            ////var checkMember = Member.GetMemberFromEmail(model.EmailAddress);
            var checkMember = Services.MemberService.GetByEmail(model.EmailAddress);

            //Check the member exists
            if (checkMember == null)
            {
                ModelState.AddModelError("LoginViewModel", "Please verify that you entered the registered email and correct password (is CAPS LOCK on? Password are Case Sensitive), or Call 877-839-2675.");

                return CurrentUmbracoPage();
                // return PartialView("authsurface/Login", model);
            }
            //Let's check they have verified their email address
            if (Convert.ToBoolean(checkMember.GetValue("hasVerifiedEmail")) == false)
            {
                //User has not verified their email yet
                ModelState.AddModelError("LoginViewModel", "Email account has not been verified");
                // return PartialView("authsurface/login", model);
                return CurrentUmbracoPage();
            }

            //Let's check they are been approved by admin
            if (Convert.ToBoolean(checkMember.GetValue("umbracoMemberApproved")) == false)
            {
                //User has not been approved yet
                ModelState.AddModelError("LoginViewModel", "Email account has not been approved");
                // return PartialView("authsurface/login", model);
                return CurrentUmbracoPage();
            }


            //check if the user is inactive
            var _inactive = checkMember.GetValue("isInactive");
            if (!String.IsNullOrWhiteSpace(_inactive.ToString()))
            {
                if (Convert.ToBoolean(_inactive) == true)
                {
                    //User has Inactivated
                    ModelState.AddModelError("LoginViewModel", "Email account has been Inactivated. Please contact support.");
                    // return PartialView("authsurface/login", model);
                    return CurrentUmbracoPage();
                }
            }

            //Let's check if the user is locked
            if (Convert.ToBoolean(checkMember.GetValue("umbracoMemberLockedOut")) == true)
            {
               
                //check if the user is locked for 15 mins.
                DateTime start = DateTime.Parse(checkMember.GetValue("umbracoMemberLastLockoutDate").ToString());

                TimeSpan timeDiff = DateTime.Now - start;
                if (timeDiff.TotalMinutes < 15)
                {
                    ModelState.AddModelError("LoginViewModel", "Email account has been Locked. Please try after 15 minutes.");
                    return CurrentUmbracoPage();
                }

                //IF the user is locked.. the user can not login
                // So unlock it..

                //the time is more than 15 min ellapsed. so unlock 
       
                checkMember.SetValue("umbracoMemberLockedOut", false);
                checkMember.SetValue("umbracoMemberLastLockoutDate", string.Empty);
                checkMember.SetValue("umbracoMemberFailedPasswordAttempts", string.Empty);
                //save the value
                ApplicationContext.Current.Services.MemberService.Save(checkMember);

            }




            //Try and login the user...

            //IF the user is locked.. the user can not login
            // so unlock is done before this..

            if (Membership.ValidateUser(model.EmailAddress, model.Password))
            {
                //Valid credentials


                //check if the user has temp email is checked..
                //if the user has the temp email redirect to ForceChange Password.
                var _mustchange = checkMember.GetValue("mustchangepassword");
                if (!String.IsNullOrWhiteSpace(_mustchange.ToString()))
                {
                    if (Convert.ToBoolean(_mustchange) == true)
                    {
                        string red = "/ForceResetPasswordPage?email=" + model.EmailAddress;
                        return Redirect(red);
                    }
                }


                //Update number of logins counter
                int noLogins = 0;

                //if the value is null assign "0"
                var _login = (checkMember.GetValue("numberOfLogins") ?? "0").ToString();
                if (int.TryParse(_login.ToString(), out noLogins))
                {
                    //Managed to parse it to a number
                    //Don't need to do anything as we have default value of 0
                }


                
                //Update the counter
                checkMember.SetValue("numberOfLogins", (noLogins + 1));

                //Update label with last login date to now
                checkMember.SetValue("lastLoggedIn", DateTime.Now.ToString());

                //save the value
                ApplicationContext.Current.Services.MemberService.Save(checkMember);


                //If they have verified then lets log them in
                //Set Auth cookie
                FormsAuthentication.SetAuthCookie(model.EmailAddress, true);

                //Once logged in  

                //WE still can not use System.Web.Security.Roles.GetRolesForUser(User.Identity.Name) 
                //  as System.Web.HttpContext.Current.User is not populated still
                // so we use model.EmailAddress
                //check if they are in these three role and redirt to approve page


                //New logic redirect to featurpage for all but shiptraining
                //check all the roles and redirect to the page.

                string[] roles = System.Web.Security.Roles.GetRolesForUser(model.EmailAddress);

                if (roles.Contains("shipcenter") || roles.Contains("shipadmin") || roles.Contains("shipdirector"))
                    return Redirect("/feature");

                if (roles.Contains("shiptraining")) //training guy redirect to counselor-training page
                    return Redirect("/counselor-training");

                if (roles.Contains("shipcounselor") || roles.Contains("shipstaff") || roles.Contains("partner") || roles.Contains("acladmin"))  //regular users show them feature page
                    return Redirect("/feature");

                if (model.ReturnUrl.IsNullOrWhiteSpace() || model.ReturnUrl.Contains("login"))
                    return Redirect("/");
                else
                    return new RedirectResult(model.ReturnUrl);



            }
            else
            {
                //user can not be logged in
                ModelState.AddModelError("LoginViewModel", "Invalid details");

                return CurrentUmbracoPage();

            }

        }

        //Used with an ActionLink
        //@Html.ActionLink("Logout", "Logout", "AuthSurface")
        public ActionResult Logout()
        {
            //Member already logged in, lets log them out and redirect them home
            if (IsLoggedOn())
            {
                Session.Clear();
                //Log member out
                FormsAuthentication.SignOut();

                //Redirect home
                return Redirect("/");
            }
            else
            {
                //Redirect home
                return Redirect("/");
            }
        }


        /// <summary>
        /// Renders the Forgotten Password view
        /// @Html.Action("RenderForgottenPassword","AuthSurface");
        /// </summary>
        /// <returns></returns>
        public ActionResult RenderForgottenPassword()
        {

            return PartialView("authsurface/ForgottenPassword", new ForgottenPasswordViewModel());
        }

        public bool IsLoggedOn()
        {
            return System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HandleForgottenPassword(ForgottenPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // return PartialView("ForgottenPassword", model);
                return CurrentUmbracoPage();

            }

            //remove the extra space if there 
            model.EmailAddress = model.EmailAddress.Trim();


            //Get member from email
            var findMember = ApplicationContext.Current.Services.MemberService.GetByEmail(model.EmailAddress);

            if (findMember != null)
            {


                //Let's check they have verified their email address
                if (Convert.ToBoolean(findMember.GetValue("hasVerifiedEmail")) == false)
                {
                    //User has not verified their email yet
                    ModelState.AddModelError("ForgottenPasswordViewModel", "Email account has not been verified");
                    // return PartialView("authsurface/login", model);
                    return CurrentUmbracoPage();
                }

                //Let's check they are been approved by admin
                if (Convert.ToBoolean(findMember.GetValue("umbracoMemberApproved")) == false)
                {
                    //User has not been approved yet
                    ModelState.AddModelError("ForgottenPasswordViewModel", "Email account has not been approved");
                    // return PartialView("authsurface/login", model);
                    return CurrentUmbracoPage();
                }

                //check if the user is inactive
                var _inactive = findMember.GetValue("isInactive");
                if (!String.IsNullOrWhiteSpace(_inactive.ToString()))
                {
                    if (Convert.ToBoolean(_inactive) == true)
                    {
                        //User has not been approved yet
                        ModelState.AddModelError("ForgottenPasswordViewModel", "Email account has been Inactivated. Please contact support.");
                        // return PartialView("authsurface/login", model);
                        return CurrentUmbracoPage();
                    }
                }


                //We found the member with that email

                //Set expiry date to 
                DateTime expiryTime = DateTime.Now.AddHours(ConfigUtil.ResetPasswordTimeOutInHours);

                //Lets update resetGUID property
                findMember.SetValue("resetGuid", expiryTime.ToString("ddMMyyyyHHmmssFFFF"));

                //Save the member with the up[dated property value
                ApplicationContext.Current.Services.MemberService.Save(findMember);

                //Send user an email to reset password with GUID in it
                EmailHelper email = new EmailHelper();


                if (email.SendPasswordResetEmail(findMember.Email, expiryTime.ToString("ddMMyyyyHHmmssFFFF")) == true)
                {
                    TempData["success"] = true;
                    return CurrentUmbracoPage();
                }
                {
                    //show  error in  page email couldnot send
                    //Return the view...
                    TempData["success"] = true;

                    TempData["message"] = "The email could not be sent.";

                }

            }
            else
            {
                ModelState.AddModelError("ForgottenPasswordViewModel", "Either you have not registered with SHIPtac or you have entered a wrong email");

            }


            return CurrentUmbracoPage();
        }


        /// <summary>
        /// Renders the Reset Password View
        /// @Html.Action("RenderResetPassword","AuthSurface");
        /// </summary>
        /// <returns></returns>
        /// 
        [ChildActionOnly]
        public ActionResult RenderResetPassword()
        {
            ResetPasswordViewModel model = new ResetPasswordViewModel();
            model.EmailAddress = Request.QueryString["email"];
            return PartialView("authsurface/ResetPassword", model);
        }


        //this page is called as a redirect to ForceResetPassword
        public ActionResult RenderForceResetPassword()
        {
            ResetPasswordViewModel model = new ResetPasswordViewModel();
            model.EmailAddress = Request.QueryString["email"];

            //do a check.. if the accoutn is locked..
            //Get member from email

            //Get member from email
            var resetMember = ApplicationContext.Current.Services.MemberService.GetByEmail(model.EmailAddress);

            //Ensure we have that member and has mustchange password set.
            //removed to check it originated from login page..
            if (resetMember != null && (resetMember.GetValue("mustchangepassword").ToString() == "1")
                && (Request.UrlReferrer != null) && (Request.UrlReferrer.AbsoluteUri != null)
                )
            {
                return PartialView("authsurface/changepass", model);
            }
            else //there is nothing so a not auth
            {
                return PartialView("authsurface/notauth");
            }



        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HandleResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // return PartialView("ResetPassword", model);
                return CurrentUmbracoPage();
            }

            //Get member from email
            var resetMember = ApplicationContext.Current.Services.MemberService.GetByEmail(model.EmailAddress);

            //Ensure we have that member
            if (resetMember != null)
            {
                //Get the querystring GUID
                var resetQS = Request.QueryString["guid"];

                //Ensure we have a vlaue in QS
                if (!string.IsNullOrEmpty(resetQS))
                {
                    //See if the QS matches the value on the member property
                    if ((resetMember.GetValue("resetGuid")??"null").ToString() == resetQS)
                    {

                        //Got a match, now check to see if the 15min window hasnt expired
                        DateTime expiryDateTime = DateTime.ParseExact(resetQS, "ddMMyyyyHHmmssFFFF", null);

                        //Check the current time is less than the expiry time
                        DateTime currentDateTime = DateTime.Now;

                        //Check if date has NOT expired (been and gone)
                        if (currentDateTime.CompareTo(expiryDateTime) < 0)
                        {

                            //Remove the resetGUID value
                            resetMember.SetValue("resetGuid", string.Empty);


                            //8/22/2017 
                            //set the islocked to false 
                            //reset lockout date
                            //set the failed password attemtpt

                            // this is neeede if the user is already locked and trying to reset the passwrod
                            resetMember.SetValue("umbracoMemberLockedOut", false);
                            resetMember.SetValue("umbracoMemberLastLockoutDate", string.Empty);
                            resetMember.SetValue("umbracoMemberFailedPasswordAttempts", string.Empty);
 

                            //Save changes and change pwd
                            ApplicationContext.Current.Services.MemberService.Save(resetMember);
                            ApplicationContext.Current.Services.MemberService.SavePassword(resetMember, model.Password);

                            TempData["success"] = true;
                            return CurrentUmbracoPage();

                            //return Redirect("/login");
                        }
                        else
                        {
                            //ERROR: Reset GUID has expired
                            ModelState.AddModelError("ResetPasswordForm.", "Reset GUID has expired");
                            return CurrentUmbracoPage();
                        }
                    }
                    else
                    {
                        //ERROR: QS does not match what is stored on member property
                        //Invalid GUID
                        ModelState.AddModelError("ResetPasswordForm.", "Invalid GUID");
                        return CurrentUmbracoPage();
                    }
                }
                else
                {
                    //ERROR: No QS present
                    //Invalid GUID
                    ModelState.AddModelError("ResetPasswordForm.", "Invalid GUID");
                    return CurrentUmbracoPage();
                }
            }


            return CurrentUmbracoPage();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HandleForceResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // return PartialView("ResetPassword", model);
                return CurrentUmbracoPage();
            }

            //Get member from email
            var resetMember = ApplicationContext.Current.Services.MemberService.GetByEmail(model.EmailAddress);

            //Ensure we have that member and has mustchange password set.
            if (resetMember != null && (resetMember.GetValue("mustchangepassword").ToString() == "1"))
            {

                //Got a match, we can allow user to update password
                //// resetMember.Password = model.Password;

                //not working..
                //resetMember.ChangePassword(model.Password);

                //remove the checkbox
                resetMember.SetValue("mustchangepassword", false);

                //reset the password related values 
                resetMember.SetValue("umbracoMemberLockedOut", false);
                resetMember.SetValue("umbracoMemberLastLockoutDate", string.Empty);
                resetMember.SetValue("umbracoMemberFailedPasswordAttempts", string.Empty);

                //Save changes
                ApplicationContext.Current.Services.MemberService.Save(resetMember);
                ApplicationContext.Current.Services.MemberService.SavePassword(resetMember, model.Password);

                //if Member is somehow logged in, lets log them out
                if (IsLoggedOn())
                {
                    Session.Clear();
                    //Log member out
                    FormsAuthentication.SignOut();
                }

                TempData["success"] = true;
                return CurrentUmbracoPage();

                //return Redirect("/login");

            }
            else
            {
                //ERROR: QS does not match what is stored on member property
                //Invalid GUID
                ModelState.AddModelError("ResetPasswordForm.", "Invalid email");
                return CurrentUmbracoPage();
            }
            return CurrentUmbracoPage();


        }



        public ActionResult RenderAddUser()
        {
            bool hasloggedin = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!hasloggedin)
            {
                return PartialView("authsurface/notauth");
            }

            //if the user in in the following role return unauthorized
            if (System.Web.HttpContext.Current.User.IsInRole("shiptraining")
             || System.Web.HttpContext.Current.User.IsInRole("shipcounselor")
             || System.Web.HttpContext.Current.User.IsInRole("shipstaff")
             || System.Web.HttpContext.Current.User.IsInRole("partner")

             ) return PartialView("authsurface/notauth");

            var userService = ApplicationContext.Current.Services.MemberService;
            string usern = string.Empty;
            usern = System.Web.HttpContext.Current.User.Identity.Name;
            var amember = userService.GetByEmail(usern);
            string astate = amember.GetValue("state").ToString();

            RegisterViewModel model = new RegisterViewModel();

            //TODO check  the role of creator 
            //if admin then use sate/ othe wise if shipadmin do not populate this.
            model.State = astate;

            return PartialView("authsurface/useradd", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HandleAddUser(RegisterViewModel model)
        {


            if (!ModelState.IsValid)
            {
                //return to current page 
                return CurrentUmbracoPage();
            }

            //check if it is duplicate email id
            //Get member from email
            var aMemeber = Services.MemberService.GetByEmail(model.EmailAddress);

            if (aMemeber != null)
            {
                ModelState.AddModelError("RegisterViewModel", "This email is already in use. ");
                return CurrentUmbracoPage();
            }


            //sammit-start


            if (PasswordValidator.DoesTextContainsFirstLastName(model.Password.Trim().ToLower(), model.FirstName.Trim().ToLower(), model.LastName.Trim().ToLower()))
            {
                ModelState.AddModelError("RegisterViewModel", "The entered password contains either FirstName/MiddleName/LastName and is not allowed.");
                return CurrentUmbracoPage();
            }

            if (PasswordValidator.DoesPassWordContainsEmail(model.EmailAddress.Trim().ToLower(), model.Password.Trim().ToLower()))
            {

                ModelState.AddModelError("RegisterViewModel", "The entered password contains your email-id and is not allowed.");
                return CurrentUmbracoPage();
            }

            if (PasswordValidator.DoesContainFourConsecutive(model.Password.Trim().ToLower()))
            {

                ModelState.AddModelError("RegisterViewModel", "The entered password contains 4 consecutive letter/number and is not allowed.");
                return CurrentUmbracoPage();
            }
            //sammit-end


            string name = model.FirstName + " " + model.LastName;
            //IMember newMember(string username, string email, string name, string memberTypeAlias);
            Umbraco.Core.Models.IMember newMember = ApplicationContext.Current.Services.MemberService.CreateMember(model.EmailAddress, model.EmailAddress, name, "Member");

            try
            {
                //Set the user Approved to true - as this is a  created by the admins
                newMember.SetValue("umbracoMemberApproved", true);


                //set all the membership properties.
                newMember.SetValue("firstName", model.FirstName);
                newMember.SetValue("lastName", model.LastName);
                newMember.SetValue("city", model.City);
                newMember.SetValue("organization", model.Organization);

                newMember.SetValue("reason", model.Reason);

                newMember.SetValue("phone", model.Phone);
                newMember.SetValue("state", model.State);

                UiEnum.ShipRolesUmbraco valueEnum = (UiEnum.ShipRolesUmbraco)Enum.Parse(typeof(UiEnum.ShipRolesUmbraco), model.Role, true);

                newMember.SetValue("roleAppliedFor", (int)valueEnum);


                newMember.SetValue("roleAssigned", (int)valueEnum);

                //Set the verified email to true
                newMember.SetValue("hasVerifiedEmail", true);

                //01/06/2017 - Set the public checkbox ..
                newMember.SetValue("addToDistributionList", model.AddToDistributionList);


                //11/2/2016 also populate the isDenied and isInactive
                newMember.SetValue("isDenied", false);
                newMember.SetValue("isInactive", false);


                //no need to create the guid as user is not going to verify his email

                //Set the Joined Date label on the member
                newMember.SetValue("joinedDate", DateTime.Now.ToString());

                newMember.SetValue("dateApproved", DateTime.Now.ToString());

                //save the value and update pwd
                ApplicationContext.Current.Services.MemberService.Save(newMember);
                ApplicationContext.Current.Services.MemberService.SavePassword(newMember, model.Password);

                //assign the  role
                ApplicationContext.Current.Services.MemberService.AssignRole(newMember.Id, model.Role);
                //umbracoMemberId = newMember.Id;


                //always set the       
                TempData["success"] = true;


                //Send out the email to the userr
                EmailHelper email = new EmailHelper();
                if (email.SendEmailToUserApprovedByShipAdmin(model.EmailAddress, model.FirstName) == false)
                {
                    //show  error in  page email couldnot send
                    TempData["message"] = "Could not send the email..";

                }


                
                //call rest api
                if (!CallRestAPI(model.EmailAddress, "A"))
                {
                    //add message
                    TempData["message"] = TempData["message"] + "Rest API call(add) was not successfull..";
                }
               

                return CurrentUmbracoPage();

            }
            catch (Exception ex)
            {

                //error management

                //Return the view...
                TempData["success"] = false;
                return CurrentUmbracoPage();
            }



        }


        /// <summary>
        /// Renders the Register View
        /// @Html.Action("RenderRegister","AuthSurface");
        /// </summary>
        /// <returns></returns>
        public ActionResult RenderRegister()
        {
            RegisterViewModel modelReg = new RegisterViewModel();

            return PartialView("Register", modelReg);
        }





        public class RecaptchaResult
        {
            [JsonProperty("success")]
            public bool Success { get; set; }

            [JsonProperty("error-codes")]
            public string[] ErrorCodes { get; set; }
        }

        [HttpPost]

        [ValidateAntiForgeryToken]
        public ActionResult HandleRegister(RegisterViewModel model)
        {



            var captchaResponse = Request.Form["g-Recaptcha-Response"];
            var webclient = new WebClient();

            //private static readonly string _recaptchaSecret = ConfigurationManager.AppSettings["Recaptcha.Secret"];

            //dev box secret key
            //string _recaptchaSecret = "6LeoEwETAAAAAKBpaOeCTWK-sEXBaDIO-wji5bvb";

            //prod box secret key
            // string _recaptchaSecret = "6Le1swYTAAAAAMzOvmtm_Y5RT62EQeZPVuePWDme";

            string _recaptchaSecret = ConfigUtil.GoogleRecaptchaSecret;

            var response = webclient.DownloadString(
                string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}&remoteip={2}",
                _recaptchaSecret,
                captchaResponse,
                Request.UserHostAddress
            ));

            var result = JsonConvert.DeserializeObject<RecaptchaResult>(response);

            if (result.Success == false)
            {
                ModelState.AddModelError("RegisterViewModel", "An error occurred validating the captcha: ");
                return CurrentUmbracoPage();
            }


            if (!ModelState.IsValid)
            {
                //return to current page 
                return CurrentUmbracoPage();
            }

            //check if it is duplicate email id
            //Get member from email
            var aMemeber = Services.MemberService.GetByEmail(model.EmailAddress);

            if (aMemeber != null)
            {
                ModelState.AddModelError("RegisterViewModel", "This email is already in use. ");
                return CurrentUmbracoPage();
            }


            //sammit-start


            if (PasswordValidator.DoesTextContainsFirstLastName(model.Password.Trim().ToLower(), model.FirstName.Trim().ToLower(), model.LastName.Trim().ToLower()))
            {
                ModelState.AddModelError("RegisterViewModel", "The entered password contains either FirstName/MiddleName/LastName and is not allowed.");
                return CurrentUmbracoPage();
            }

            if (PasswordValidator.DoesPassWordContainsEmail(model.EmailAddress.Trim().ToLower(), model.Password.Trim().ToLower()))
            {

                ModelState.AddModelError("RegisterViewModel", "The entered password contains your email-id and is not allowed.");
                return CurrentUmbracoPage();
            }

            if (PasswordValidator.DoesContainFourConsecutive(model.Password.Trim().ToLower()))
            {

                ModelState.AddModelError("RegisterViewModel", "The entered password contains 4 consecutive letter/number and is not allowed.");
                return CurrentUmbracoPage();
            }
            //sammit-end


            //Model valid let's create the member
            try
            {
                string name = model.FirstName + " " + model.LastName;
                Umbraco.Core.Models.IMember newMember = ApplicationContext.Current.Services.MemberService.CreateMember(model.EmailAddress, model.EmailAddress, name, "Member");

                //Set the user Approved to false
                newMember.SetValue("umbracoMemberApproved", false);

                //set all the membership properties.
                newMember.SetValue("firstName", model.FirstName);
                newMember.SetValue("lastName", model.LastName);
                newMember.SetValue("city", model.City);
                newMember.SetValue("organization", model.Organization);

                newMember.SetValue("reason", model.Reason);

                newMember.SetValue("phone", model.Phone);
                newMember.SetValue("state", model.State);


                if (model.Role == "shiptrainingother") model.Role = "shiptraining";

                UiEnum.ShipRolesUmbraco valueEnum = (UiEnum.ShipRolesUmbraco)Enum.Parse(typeof(UiEnum.ShipRolesUmbraco), model.Role, true);

                newMember.SetValue("roleAppliedFor", (int)valueEnum);



                //Set the verified email to false
                newMember.SetValue("hasVerifiedEmail", false);

                newMember.SetValue("isDenied", false);
                newMember.SetValue("isInactive", false);

                //11/2/2016  populate the roleAssigned with a zero so that it is needed when aproving it 
                //also will be used for search..
                newMember.SetValue("roleAssigned", "0");


                //Create temporary GUID
                var tempGUID = Guid.NewGuid();

                //Set the verification email GUID value on the member
                newMember.SetValue("emailVerifyGuid", tempGUID.ToString());

                //Set the Joined Date label on the member
                newMember.SetValue("joinedDate", DateTime.Now.ToString());

                //Save changes
                ApplicationContext.Current.Services.MemberService.Save(newMember);
                ApplicationContext.Current.Services.MemberService.SavePassword(newMember, model.Password);


                //Do not assign a role


                //Send out verification email, with GUID in it 
                //8/17/2017 add the emailID so that when verifying the userID can be used for faster seek
                EmailHelper email = new EmailHelper();
                //if (email.SendVerifyEmail(model.EmailAddress, model.FirstName, tempGUID.ToString()) == false)
                if (email.SendVerifyEmail(model.EmailAddress, model.FirstName, tempGUID.ToString()) == false)
                {
                    //show  error in  page email couldnot send

                    //Return the view...
                    TempData["success"] = false;
                    ///////  return CurrentUmbracoPage();
                }



                if (email.SendDesignatedNotificationForRegisterUser(model) == false)
                {
                    //show  error in  page email couldnot send
                    //Return the view...
                    TempData["success"] = false;
                    return CurrentUmbracoPage();

                }




            }
            catch (Exception ex)
            {
                //EG: Duplicate email address - already exists
                //error management

                //Return the view...
                TempData["success"] = false;
                return CurrentUmbracoPage();
            }




            //Return the view...
            TempData["success"] = true;
            return CurrentUmbracoPage();

        }



        /// <summary>
        /// Renders the Verify Email
        /// @Html.Action("RenderVerifyEmail","AuthSurface");
        /// 
        /// //sammit this function is available at my controller.
        /// </summary>
        /// <returns></returns>

        public ActionResult RenderVerifyEmail(string e, string v)
        {

            //search the member by email.
            // if found and if the hasVerifiedEmail not set - set it to true
            var findMember = ApplicationContext.Current.Services.MemberService.GetByEmail(e);
            //Ensure we find a member with the verifyGUID
            if (findMember != null)
            {
                //We got the member

                //we will check if the value is already set then no need to set it again...
                if (Convert.ToBoolean(findMember.GetValue("hasVerifiedEmail")) == false)
                {

                    //so let's update the verify email checkbox
                    findMember.SetValue("hasVerifiedEmail", true);

                    //Save changes
                    ApplicationContext.Current.Services.MemberService.Save(findMember);
                }

            }
            else
            {
                //Couldn't find them - most likely invalid GUID
                return Redirect("/");
            }

            //Saved the content redirect to success page.
            return Redirect("/success");
        }


        //REMOTE Validation  - I think not used
        /// <summary>
        /// Used with jQuery Validate to check when user registers that email address not already used
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        public JsonResult CheckEmailIsUsed(string emailAddress)
        {
            //Try and get member by email typed in
            var checkEmail = Services.MemberService.GetByEmail(emailAddress);

            if (checkEmail != null)
            {
                return Json(String.Format("The email address '{0}' is already in use.", emailAddress), JsonRequestBehavior.AllowGet);
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }



        public ActionResult RenderUserApproval(string id)
        {
            bool hasloggedin = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!hasloggedin)
            {
                return PartialView("authsurface/notauth");
            }

            //if the user in in the following role return unauthorized
            if (System.Web.HttpContext.Current.User.IsInRole("shiptraining")
             || System.Web.HttpContext.Current.User.IsInRole("shipcounselor")
             || System.Web.HttpContext.Current.User.IsInRole("shipstaff")
             || System.Web.HttpContext.Current.User.IsInRole("partner")

             ) return PartialView("authsurface/notauth");

            UserApprovalViewModel model = new UserApprovalViewModel();

            int _id = Convert.ToInt32(id);


            //get the pending user and populate the model

            var Imember = Services.MemberService.GetById(_id);

            model.ID = _id;

            model.IsUserApproved = Imember.IsApproved;

            model.IsUserVerifiedEmail = Convert.ToBoolean(Imember.GetValue("hasVerifiedEmail"));

            model.IsUserInactive = Convert.ToBoolean(Imember.GetValue("isInactive"));

            model.EmailAddress = Imember.Email;

            model.FirstName = Imember.GetValue("firstName").ToString();
            model.LastName = Imember.GetValue("lastName").ToString();


            model.Phone = Imember.GetValue("phone").ToString();
            model.State = Imember.GetValue("state").ToString();

            model.City = Imember.GetValue("city").ToString();

            model.Organization = Imember.GetValue("organization").ToString();

            model.Reason = Imember.GetValue("reason").ToString();

            model.AddToDistributionList = Convert.ToBoolean(Imember.GetValue("addToDistributionList"));

            string roleNo = Imember.GetValue("roleAppliedFor").ToString();
            UiEnum.ShipRolesUmbraco valueEnum = (UiEnum.ShipRolesUmbraco)Enum.Parse(typeof(UiEnum.ShipRolesUmbraco), roleNo, true);


            model.RoleApplied = valueEnum.Description();

            var val = Imember.GetValue("roleAssigned");
            string roleAssignedNo = null;

            //if the value is not null
            if (val != null)
            {
                roleAssignedNo = val.ToString();
            }

            if (String.IsNullOrWhiteSpace(roleAssignedNo))
            {
                model.RoleAssigned = "";
            }

            else
            {
                UiEnum.ShipRolesUmbraco valueEnum1 = (UiEnum.ShipRolesUmbraco)Enum.Parse(typeof(UiEnum.ShipRolesUmbraco), roleAssignedNo, true);
                model.RoleAssigned = valueEnum1.Description();

            }


            return PartialView("authsurface/UserApproval", model);
        }


        public ActionResult RenderUserChangeRole(string id)
        {
            bool hasloggedin = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!hasloggedin)
            {
                return PartialView("authsurface/notauth");
            }

            //if the user in in the following role return unauthorized
            if (System.Web.HttpContext.Current.User.IsInRole("shiptraining")
             || System.Web.HttpContext.Current.User.IsInRole("shipcounselor")
             || System.Web.HttpContext.Current.User.IsInRole("shipstaff")
             || System.Web.HttpContext.Current.User.IsInRole("partner")

             ) return PartialView("authsurface/notauth");

            UserApprovalViewModel model = new UserApprovalViewModel();

            int _id = Convert.ToInt32(id);


            //get the pending user and populate the model

            var Imember = Services.MemberService.GetById(_id);

            model.ID = _id;

            model.IsUserApproved = Imember.IsApproved;

            model.IsUserVerifiedEmail = Convert.ToBoolean(Imember.GetValue("hasVerifiedEmail"));

            model.IsUserInactive = Convert.ToBoolean(Imember.GetValue("isInactive"));

            model.EmailAddress = Imember.Email;

            model.FirstName = Imember.GetValue("firstName").ToString();
            model.LastName = Imember.GetValue("lastName").ToString();


            model.Phone = Imember.GetValue("phone").ToString();
            model.State = Imember.GetValue("state").ToString();

            model.City = Imember.GetValue("city").ToString();

            model.Organization = Imember.GetValue("organization").ToString();

            model.Reason = Imember.GetValue("reason").ToString();

            model.AddToDistributionList = Convert.ToBoolean(Imember.GetValue("addToDistributionList"));

            string roleNo = Imember.GetValue("roleAppliedFor").ToString();
            UiEnum.ShipRolesUmbraco valueEnum = (UiEnum.ShipRolesUmbraco)Enum.Parse(typeof(UiEnum.ShipRolesUmbraco), roleNo, true);


            model.RoleApplied = valueEnum.Description();


            string roleAssignedNo = Imember.GetValue("roleAssigned").ToString();
            if (String.IsNullOrWhiteSpace(roleAssignedNo))
            {
                model.RoleAssigned = "";
            }
            else
            {
                UiEnum.ShipRolesUmbraco valueEnum1 = (UiEnum.ShipRolesUmbraco)Enum.Parse(typeof(UiEnum.ShipRolesUmbraco), roleAssignedNo, true);
                model.RoleAssigned = valueEnum1.Description();

            }


            return PartialView("authsurface/UserChangeRole", model);
        }

        private bool CallRestAPI(string useremail, string action)
        {

            string queryString = Helpers.GetUserDetailAction(useremail, action);

            string URL = "http://shipta.medicareinteractive.org/verify.php";
            string urlParameters = "?q=" + queryString;

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            // List data response.
            HttpResponseMessage response = client.GetAsync(urlParameters).Result;
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HandleUserApproval(UserApprovalViewModel model)
        {
            //this module is used to 
            //1. approve
            //2. Deny
            //3. Save
            //4. resend email, etc...

            //var Imember = Services.MemberService.GetByEmail(model.EmailAddress);

            var Imember = Services.MemberService.GetById(model.ID);

            //Ensure we find a member with the ID
            if (Imember == null)
            {
                //add error
                ModelState.AddModelError("UserApprovalViewModel", "Could not finde the user..");
                TempData["success"] = false;
                return RedirectToCurrentUmbracoPage();
            }
            //check what button pressed
            if (model.SubmitAction == "Approve" || model.SubmitAction == "OverRideEmailApprove")
            {

                UiEnum.ShipRolesUmbraco valueEnum = (UiEnum.ShipRolesUmbraco)Enum.Parse(typeof(UiEnum.ShipRolesUmbraco), model.Role, true);

                // Services.MemberService.AssignRole(Imember.Email, "RegisteredMembers");

                Services.MemberService.AssignRole(Imember.Email, model.Role);
                Imember.IsApproved = true;
                Imember.SetValue("dateApproved", DateTime.Now.ToString());


                // update hasVerifiedEmail if the button OverRideEmailand Approve button  is pressed.
                if (model.SubmitAction == "OverRideEmailApprove")
                    Imember.SetValue("hasVerifiedEmail", true);

                Imember.SetValue("addToDistributionList", model.AddToDistributionList);

                //set the correct role here..
                Imember.SetValue("roleAssigned", (int)valueEnum);

                Services.MemberService.Save(Imember);


                //Send user an email to reset password with GUID in it
                EmailHelper email = new EmailHelper();


                if (email.SendApprovedEmail(model.EmailAddress, Imember.Name))
                {

                    TempData["success"] = true;
                    //show  messge
                    TempData["message"] = "Your action was successfull.";

                }
                else //approved email could not send..
                {
                    //add error
                    ModelState.AddModelError("UserApprovalViewModel", "Could not send the email..");
                    TempData["success"] = false;

                }

                //call rest api
                if (!CallRestAPI(model.EmailAddress, "A"))
                {
                    //add error
                    ModelState.AddModelError("UserApprovalViewModel", "Rest API call was not successfull..");
                }

                return RedirectToCurrentUmbracoPage();


            }

            else if (model.SubmitAction == "Deny")
            {

                //NO need to delte this member..We want to see the history
                // Services.MemberService.Delete(Imember);
                Imember.IsApproved = false;
                Imember.SetValue("isDenied", "1");  //0 false 1 true

                Imember.SetValue("dateDenied", DateTime.Now.ToString());
                Services.MemberService.Save(Imember);

                //Send user an email about deny
                EmailHelper email = new EmailHelper();

                if (email.SendDisapproveEmail(model.EmailAddress, Imember.Name))
                {

                    //add succes
                    TempData["success"] = true;
                    //show  messge
                    TempData["message"] = "Your action was successfull.";
                    return RedirectToCurrentUmbracoPage();
                }
                else //deny email could not send..
                {
                    //add error
                    ModelState.AddModelError("UserApprovalViewModel", "Could not send the email..");
                    TempData["success"] = false;
                    return RedirectToCurrentUmbracoPage();
                }



            }
            else if (model.SubmitAction == "Save")
            {

                if (!ModelState.IsValid)
                {
                    // return PartialView("ResetPassword", model);
                    return CurrentUmbracoPage();
                }

                //save button do not change the role..

                //check if the 
                if (!model.EmailAddressChange.IsNullOrWhiteSpace())
                {
                    //check if it is duplicate email id
                    //Get member from email
                    var aMemeber = Services.MemberService.GetByEmail(model.EmailAddressChange);

                    if (aMemeber != null)
                    {
                        ModelState.AddModelError("UserApprovalViewModel", "This email is already in use. Please try another email to edit.");
                        return CurrentUmbracoPage();
                    }


                    //save the email and username
                    Imember.Email = model.EmailAddressChange;
                    Imember.Username = model.EmailAddressChange;

                }


                Imember.SetValue("firstName", model.FirstName);
                Imember.SetValue("lastName", model.LastName);
                Imember.SetValue("city", model.City);
                Imember.SetValue("organization", model.Organization);
                Imember.SetValue("reason", model.Reason);

                Imember.SetValue("phone", model.Phone);
                Imember.SetValue("state", model.State);

                Imember.SetValue("addToDistributionList", model.AddToDistributionList);

                Services.MemberService.Save(Imember);

                if (model.EmailAddressChange.IsNullOrWhiteSpace())
                {
                    CallRestAPI(model.EmailAddress, "E");
                }
                else
                {
                    CallRestAPI(model.EmailAddressChange, "E");

                }


                TempData["success"] = true;

                //show  messge
                TempData["message"] = "Your edits have been saved.";

                return CurrentUmbracoPage();

            }

            else if (model.SubmitAction == "ResendEmail")
            {
                //send the user as an email for verification again.


                //Create temporary GUID
                var tempGUID = Guid.NewGuid();   //Create temporary GUID

                Imember.SetValue("emailVerifyGuid", tempGUID.ToString());
                Services.MemberService.Save(Imember);


                //Send out verification email, with GUID in it
                EmailHelper email = new EmailHelper();
                if (email.SendVerifyEmail(Imember.Email, model.FirstName, tempGUID.ToString()))
                {
                    //add succes
                    TempData["success"] = true;
                    //show  messge
                    TempData["message"] = "Your action was successfull.";
                    return RedirectToCurrentUmbracoPage();
                }
                else //deny email could not send..
                {
                    //add error
                    ModelState.AddModelError("UserApprovalViewModel", "Could not send the resend email verification..");
                    TempData["success"] = false;
                    return RedirectToCurrentUmbracoPage();
                }

            }

            else if (model.SubmitAction == "Inactive")
            {
                //set the user as inactive

                Imember.SetValue("isInactive", true);

                Imember.SetValue("dateInactive", DateTime.Now.ToString());

                Services.MemberService.Save(Imember);
                //add succes
                TempData["success"] = true;
                //show  messge
                TempData["message"] = "Your action was successfull.";

                //call rest api 2/8/2018 - Inactive
                if (!CallRestAPI(model.EmailAddress, "I"))
                {
                    //add error
                    ModelState.AddModelError("UserApprovalViewModel", "Rest API call was not successfull..");
                }
                return RedirectToCurrentUmbracoPage();

            }
            else if (model.SubmitAction == "Active")
            {
                //set the user as Active

                Imember.SetValue("isInactive", false);

                //clear the inactive date :
                //Fixed 4/17/2018
                Imember.SetValue("dateInactive", string.Empty);

                //fixed 8/5/2018
                // reset the password related fields.
                Imember.SetValue("umbracoMemberLockedOut", false);
                Imember.SetValue("umbracoMemberLastLockoutDate", string.Empty);
                Imember.SetValue("umbracoMemberFailedPasswordAttempts", string.Empty);

                //save the value

                Services.MemberService.Save(Imember);

                //add succes
                TempData["success"] = true;
                //show  messge
                TempData["message"] = "Your action was successfull.";

                //call rest api 2/8/2018 - Reactivated
                if (!CallRestAPI(model.EmailAddress, "R"))
                {
                    //add error
                    ModelState.AddModelError("UserApprovalViewModel", "Rest API call was not successfull..");
                }

                return RedirectToCurrentUmbracoPage();

            }
            else if (model.SubmitAction == "Delete")
            {
                //delete  the user and show the approve page

                //call rest method first and then delte the user from database..
                //other wise the user detail is lost..
                //call rest api
                CallRestAPI(model.EmailAddress, "D");

                Services.MemberService.Delete(Imember);
                return Redirect("/approve");

            }

            else if (model.SubmitAction == "GenerateTempPassWord")
            {

                // Generate a new 12-character password with 1 non-alphanumeric character.
                string pwd = Membership.GeneratePassword(12, 4);

                //we are not allowing < and &, so we will remove if present in the pwd.
                pwd.Trim(new Char[] { '<', '&' });

                //Get member from email

                var resetMember = ApplicationContext.Current.Services.MemberService.GetByEmail(model.EmailAddress);

                resetMember.SetValue("mustchangepassword", true);

                //Fixed 8/15/2018
                //if the user is locked he will not be able to login, so unlock the account
                resetMember.SetValue("umbracoMemberLockedOut", false);
                resetMember.SetValue("umbracoMemberLastLockoutDate", string.Empty);
                resetMember.SetValue("umbracoMemberFailedPasswordAttempts", string.Empty);

                //Save changes
                ApplicationContext.Current.Services.MemberService.Save(resetMember);

                ApplicationContext.Current.Services.MemberService.SavePassword(resetMember, pwd);



                TempData["success"] = true;

                //show  messge
                TempData["message"] = "The temporary password generated is:<br /><strong>" + pwd + "</strong>";


            }


            return CurrentUmbracoPage();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HandleUserChangeRole(UserApprovalViewModel model)
        {


            //var Imember = Services.MemberService.GetByEmail(model.EmailAddress);

            var Imember = Services.MemberService.GetById(model.ID);

            //Ensure we find a member with the ID
            if (Imember == null)
            {
                //add error
                ModelState.AddModelError("UserApprovalViewModel", "Could not finde the user..");
                TempData["success"] = false;
                return RedirectToCurrentUmbracoPage();
            }
            //check what button pressed
            if (model.SubmitAction == "Approve")
            {

                UiEnum.ShipRolesUmbraco valueEnum = (UiEnum.ShipRolesUmbraco)Enum.Parse(typeof(UiEnum.ShipRolesUmbraco), model.Role, true);


                //DissociateRole if any..
                string[] roles = System.Web.Security.Roles.GetRolesForUser(Imember.Email);

                foreach (string arole in roles)
                {

                    Services.MemberService.DissociateRole(Imember.Email, arole);
                }

                //assign the new role
                Services.MemberService.AssignRole(Imember.Email, model.Role);
                Imember.IsApproved = true;


                //set the correct role here..
                Imember.SetValue("roleAssigned", (int)valueEnum);

                Services.MemberService.Save(Imember);


                //call rest api
                if (!CallRestAPI(Imember.Email, "E"))
                {
                    //add error
                    ModelState.AddModelError("UserApprovalViewModel", "Rest API call was not successfull..");
                }

                return Redirect("/approved");


            }


            else if (model.SubmitAction == "Cancel")
            {

                return Redirect("/approved");
            }

            return CurrentUmbracoPage();

        }

    }
}

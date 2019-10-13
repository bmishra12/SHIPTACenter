using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Configuration;

using UmbracoShipTac.Code;
using UmbracoShipTac.Models;
using Umbraco.Core.Services;
using Umbraco.Core;
using Umbraco.Core.Persistence.Querying;


namespace UmbracoShipTac.Code
{
    public class EmailHelper
    {

        public bool SendEmailToUserApprovedByShipAdmin(string emailId, string firstName)
        {

            //Send a reset email to member
            // Create the email object first, then add the properties.
            //Prepare Mail Object
            ShiptalkMailMessage mailMessage = new ShiptalkMailMessage(true, ShiptalkMailMessage.MailFrom.ShiptalkResourceCenter);



            //Send to the member's email address
            mailMessage.ToList.Add(emailId);

            //Subject
            if (ConfigUtil.WebEnvironment != "prod")
            {
                mailMessage.Subject = "Your shiptacenter.org registration(" + ConfigUtil.WebEnvironment + ")";

            }
            else
            {
                mailMessage.Subject = "Your shiptacenter.org registration";
            }

            mailMessage.Body = CreateEmailToUserApprovedByShipAdminBody(firstName);

            //Send Mail here
            ShiptalkMail mail = new ShiptalkMail(mailMessage);
            return mail.SendMail();

        }

        private string CreateEmailToUserApprovedByShipAdminBody(string firstName)
        {

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (ConfigUtil.WebEnvironment != "prod")
            {
                sb.AppendFormat("-----------TEST  User Registration DEV site------------"); sb.AddNewHtmlLines(2);
                sb.AppendFormat("-----------Created from DEV environment: {0}------------", ConfigUtil.WebEnvironment); sb.AddNewHtmlLines(2); ;
            }
            sb.AppendFormat("Dear {0},", firstName.ToCamelCasing());
            sb.AddNewHtmlLines(2);
            sb.Append(@"An account to access the password-protected area of portal.shiptacenter.org has been created for you.
                This website is hosted by the State Health Insurance Assistance Program National Technical Assistance 
               Center (SHIP TA Center). The password-protected area of www.shiptacenter.org houses educational 
                materials for SHIP representatives such as staff, partners, counselors, or counselors in training. SHIP is a 
                national program; however, we realize some programs have state-specific names and go by acronyms 
                such as SHIBA, APPRISE, or SHICK, to name a few.");
            sb.AddNewHtmlLines(2);
            sb.AppendLine("Please follow these instructions to access your account:");
            sb.AddNewHtmlLine();
            sb.AppendLine("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;1. Go to <a href='www.shiptacenter.org'>www.shiptacenter.org</a> and click on the orange padlock (SHIP Login)");
            sb.AddNewHtmlLine();

            sb.AppendLine("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;2. Click \"Forgot Password\" ");
            sb.AddNewHtmlLine();

            sb.AppendLine("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;3. Enter your email address");
            sb.AddNewHtmlLine();

            sb.AppendLine("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;4. A link to create your own password will arrive from info@shiptacenter.org with the ");
            sb.AddNewHtmlLine();

            sb.AppendLine("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;subject \"Your request at the shiptacenter.org\" ");
            sb.AddNewHtmlLine();

            sb.AppendLine("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;5. Follow the link to create a private password");
            sb.AddNewHtmlLine();

            sb.AppendLine("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;a. Your password must be between 8 to 30 characters and must contain at least ");
            sb.AddNewHtmlLine();

            sb.AppendLine("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;one case letter, at least one digit and at least one special character,");
            sb.AddNewHtmlLine();

            sb.AppendLine("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;with the exception of the characters < and &, which cannot be used.");
            
            sb.AddNewHtmlLines(2);

            sb.Append("If you have any questions, please contact your SHIP supervisor or the SHIP TA Center. ");
            sb.AddNewHtmlLines(2);

            sb.Append("Thank you!  ");
            sb.AddNewHtmlLines(2);

            sb.Append("The SHIP National Technical Assistance Center");
            sb.AddNewHtmlLine();
            sb.Append(ConfigUtil.EmailOfResourceCenter);
            sb.AddNewHtmlLine();
            sb.Append(ConfigUtil.ShiptalkSupportPhone);
            sb.AddNewHtmlLine();

            sb.AddNewHtmlLines(5);

            return sb.ToString();

        }

        public bool SendVerifyEmail(string emailId, string firstName, string verifyGUID)
        {
            //Send a reset email to member
            // Create the email object first, then add the properties.
            //Prepare Mail Object
            ShiptalkMailMessage mailMessage = new ShiptalkMailMessage(true, ShiptalkMailMessage.MailFrom.ShiptalkResourceCenter);



            //Send to the member's email address
            mailMessage.ToList.Add(emailId);

            //Subject
            if (ConfigUtil.WebEnvironment != "prod")
            {
                mailMessage.Subject = "Your shiptacenter.org registration(" + ConfigUtil.WebEnvironment + ")";

            }
            else
            {
                mailMessage.Subject = "Your shiptacenter.org registration";
            }

            //Verify link
            //// var verifyURL = baseURL + "/umbraco/surface/my/RenderVerifyEmail" + "?verifyGUID=" + verifyGUID;

           
            string linkFormat = "<a href='" + ConfigUtil.EmailConfirmationUrl + "?e={0}&v={1}'>Follow this link</a>";
            string confirmLink = string.Format(linkFormat, emailId, verifyGUID);
            string textlink = ConfigUtil.EmailConfirmationUrl + "?e={0}&v={1}";
            textlink = string.Format(textlink, emailId, verifyGUID);

            mailMessage.Body = CreateVerifyEmailBody(firstName, confirmLink, textlink);

            //Send Mail here
            ShiptalkMail mail = new ShiptalkMail(mailMessage);
            return mail.SendMail();
        }

        private static string CreateVerifyEmailBody(string firstName, string confirmLink, string textlink)
        {


            //Dear (first name),
            //Your request to access the “For SHIPs” area of portal.shiptacenter.org has been received. 
            //Follow this link [URL] to verify your email address and submit your registration for approval. 
            //If you have difficulties accessing the link, copy and paste the link below to your browser to verify your email address: Final URL goes here.
            //The director for your State Health Insurance Assistance Program (SHIP) will review your request. 
            //You will receive an email from info@shiptacenter.org when the director’s decision has been made. If you have not received a reply within a week, please contact info@shiptacenter.org. 
            //Sincerely,
            //The SHIP National Technical Assistance Center
            //portal.shiptacenter.org
            //877-839-2675


            //HTML Message
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (ConfigUtil.WebEnvironment != "prod")
            {
                sb.AppendFormat("-----------TEST  User Registration DEV site------------"); sb.AddNewHtmlLines(2);
                sb.AppendFormat("-----------Created from DEV environment: {0}------------", ConfigUtil.WebEnvironment); sb.AddNewHtmlLines(2); ;
            }
            sb.AppendFormat("Dear {0},", firstName.ToCamelCasing());
            sb.AddNewHtmlLines(2);
            sb.Append("Your request to access the \"For SHIPs\" area of portal.shiptacenter.org has been received.");
            sb.AddNewHtmlLines(3);
            sb.AppendFormat("{0} to verify your email address and submit your registration for approval.", confirmLink);
            sb.AppendFormat(" If you have difficulties accessing the link, copy and paste the link below to your browser to verify your email address:");
            sb.AddNewHtmlLines(2);
            sb.Append(textlink);
            sb.AddNewHtmlLines(3);
            sb.Append("The director for your State Health Insurance Assistance Program (SHIP) will review your request.");
            sb.Append(" You will receive an email from info@shiptacenter.org when the director’s decision has been made. If you have not received a reply within a week, please contact info@shiptacenter.org.");
            sb.AddNewHtmlLines(3);
            sb.Append("Sincerely,");
            sb.AddNewHtmlLines(2);
            sb.Append("The SHIP National Technical Assistance Center");
            sb.AddNewHtmlLine();
            sb.Append(ConfigUtil.ShiptalkSupportPhone);
            sb.AddNewHtmlLine();
            sb.Append(ConfigUtil.EmailOfResourceCenter);
            sb.AddNewHtmlLines(5);

            return sb.ToString();

        }


        public bool SendDesignatedNotificationForRegisterUser(RegisterViewModel model)
        {

            // var mailMessage = new MailMessage();

            //Prepare Mail Object
            ShiptalkMailMessage mailMessage = new ShiptalkMailMessage(true, ShiptalkMailMessage.MailFrom.ShiptalkResourceCenter);


            int intStateAdmin = (int)UiEnum.ShipRolesUmbraco.shipadmin;

            int intStateDir = (int)UiEnum.ShipRolesUmbraco.shipdirector;

            var userStateAdminList = ApplicationContext.Current.Services.MemberService.GetMembersByPropertyValue("RoleAssigned", intStateAdmin, ValuePropertyMatchType.Exact).Where(x => x.IsApproved);
            var userStateDirList = ApplicationContext.Current.Services.MemberService.GetMembersByPropertyValue("RoleAssigned", intStateDir, ValuePropertyMatchType.Exact).Where(x => x.IsApproved);
            var memberAdmin = userStateAdminList.Where(x => x.Properties["state"].Value.ToString() == model.State && x.Properties["isInactive"].Value.ToString() != "1");
            var membersDir = userStateDirList.Where(x => x.Properties["state"].Value.ToString() == model.State && x.Properties["isInactive"].Value.ToString() != "1");


            foreach (var member in memberAdmin)
            {

                mailMessage.ToList.Add(member.Email);

            }

            foreach (var member in membersDir)
            {

                mailMessage.ToList.Add(member.Email);

            }

            if (ConfigUtil.WebEnvironment != "prod")
            {
                mailMessage.Subject = "New shiptacenter.org(" + ConfigUtil.WebEnvironment + ")Registration";

            }
            else
            {
                mailMessage.Subject = "New shiptacenter.org Registration";
            }
            mailMessage.Body = CreateEmailBodyForRegistrationNotification(model);


            //Send Mail here
            ShiptalkMail mail = new ShiptalkMail(mailMessage);
            return mail.SendMail();


        }

        private static string CreateEmailBodyForRegistrationNotification(RegisterViewModel userAccount)
        {

            //Email to state director:
            //Dear (first name),
            //A request to become an approved user of the “For SHIPs” area of portal.shiptacenter.org has been received from (first name last name). 
            //Please review this request on your administrator dashboard at portal.shiptacenter.org to approve or deny the request.
            //Sincerely, 
            //The SHIP National Technical Assistance Center
            //portal.shiptacenter.org
            //877-839-2675

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (ConfigUtil.WebEnvironment != "prod")
            {
                sb.AppendFormat("-----------TEST User Registration from DEV site------------"); sb.AddNewHtmlLines(2);
                sb.AppendFormat("-----------Created from DEV environment: {0}------------", ConfigUtil.WebEnvironment); sb.AddNewHtmlLines(2);
            }
            sb.AppendFormat("-----------New User Registration Notification------------"); sb.AddNewHtmlLines(2);


            sb.Append("A request to become an approved user of the “For SHIPs” area of portal.shiptacenter.org has been received.");
            sb.AddNewHtmlLine();
            sb.Append("Please review this request on your administrator dashboard at portal.shiptacenter.org to approve or deny the request.");

            sb.AddNewHtmlLines(3);


            sb.Append("Here is a summary of the registration:"); sb.AddNewHtmlLines(2);

            sb.Append("<strong>Personal Information:</strong>"); sb.AddNewHtmlLine();
            sb.AppendFormat("Name: {0}", userAccount.FirstName); sb.AddNewHtmlLine();
            sb.AppendFormat("Last Name: {0}", userAccount.LastName); sb.AddNewHtmlLine();

            sb.Append("<strong>Contact Information:</strong>"); sb.AddNewHtmlLine();
            sb.AppendFormat("Primary Phone: {0}", userAccount.Phone); sb.AddNewHtmlLine();
            sb.AppendFormat("Primary Email: {0}", userAccount.EmailAddress); sb.AddNewHtmlLine();

            sb.Append("<strong>Requested Account Information:</strong>"); sb.AddNewHtmlLine();
            sb.AppendFormat("State: {0}", userAccount.State); sb.AddNewHtmlLines(2);


            //Need to add User Descriptor and Regional Access profile by verifying what role or agency was selected.

            sb.Append("Sincerely,");
            sb.AddNewHtmlLines(2);
            sb.Append("The SHIP National Technical Assistance Center");
            sb.AddNewHtmlLine();
            sb.Append(ConfigUtil.ShiptalkSupportPhone);
            sb.AddNewHtmlLine();
            sb.Append(ConfigUtil.EmailOfResourceCenter);
            sb.AddNewHtmlLines(5);


            return sb.ToString();
        }



        public bool SendApprovedEmail(string userEmail, string userName)
        {



            //Prepare Mail Object
            ShiptalkMailMessage mailMessage = new ShiptalkMailMessage(true, ShiptalkMailMessage.MailFrom.ShiptalkResourceCenter);




            mailMessage.ToList.Add(userEmail);
            mailMessage.Subject = "[External] Your shiptacenter.org account has been approved";

            mailMessage.Body = CreateEmailBodyForSendApproveEmail(userName);

            //Send Mail here
            ShiptalkMail mail = new ShiptalkMail(mailMessage);
            return mail.SendMail();
        }


        private static string CreateEmailBodyForSendApproveEmail(string userName)
        {


            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.AppendFormat("Dear {0},", userName);
            sb.AddNewHtmlLines(2);
            sb.Append("Your request to shiptacenter.org account has been approved.");
            sb.AddNewHtmlLines(2);
            sb.Append("Your registration to access the SHIP only area of <a href='www.shiptacenter.org'>www.shiptacenter.org</a> has been approved by a SHIP administrator. You may login at <a href='www.shiptacenter.org'>www.shiptacenter.org</a> using the email and password you provided upon registration. ");
            sb.AddNewHtmlLines(2);

            sb.Append("If you do not remember your password, visit <a href='www.shiptacenter.org'>www.shiptacenter.org</a>, click on the orange padlock (SHIP Login), and use the forgot your password link. Follow the instructions to have the password reset instructions emailed to you. You must respond within 72 hours or you will have to repeat the steps above. Once you reset your password, you should be able to log in to the website with your username (email address) and new password.");
            sb.AddNewHtmlLines(2);


            sb.Append("Sincerely,");
            sb.AddNewHtmlLines(2);
            sb.Append("The SHIP National Technical Assistance Center");
            sb.AddNewHtmlLine();
            sb.Append("<a href='www.shiptacenter.org'>www.shiptacenter.org</a>");
            sb.AddNewHtmlLine();
            sb.Append(ConfigUtil.ShiptalkSupportPhone);
            sb.AddNewHtmlLines(5);

            return sb.ToString();

        }


        public bool SendDisapproveEmail(string userEmail, string userName)
        {



            //Prepare Mail Object
            ShiptalkMailMessage mailMessage = new ShiptalkMailMessage(true, ShiptalkMailMessage.MailFrom.ShiptalkResourceCenter);


            mailMessage.ToList.Add(userEmail);
            mailMessage.Subject = "Your request was denied";

            mailMessage.Body = CreateEmailBodyForSendDisapproveEmail(userName);

            //Send Mail here
            ShiptalkMail mail = new ShiptalkMail(mailMessage);
            return mail.SendMail();
        }

        private static string CreateEmailBodyForSendDisapproveEmail(string userName)
        {
            //Dear (first name),
            //Your request to become an approved user of the “For SHIPs” area of portal.shiptacenter.org has been denied by the director of your State Health Insurance Assistance Program (SHIP). As a reminder, the “For SHIPs” area of portal.shiptacenter.org is intended for representatives who do SHIP work. To visit the area of our website that is available to the general public, click here.
            //Sincerely, 
            //The SHIP National Technical Assistance Center
            //portal.shiptacenter.org
            //877-839-2675

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.AppendFormat("Dear {0},", userName);
            sb.AddNewHtmlLines(2);
            sb.Append("Your request for an account to access the SHIP only area of portal.shiptacenter.org was denied by your SHIP administrator. The login area of portal.shiptacenter.org is intended only for State health Insurance Assistance Program (SHIP) representatives. If you are a member of the general public and wish to learn more about the SHIP program, visit our home page. If you are a SHIP representative and think your request was denied in error, contact a SHIP administrator to inquire. A SHIP locator is available on our home page.");
            sb.AddNewHtmlLines(2);
            sb.Append("Sincerely, ");
            sb.AddNewHtmlLine();
            sb.Append("The SHIP National Technical Assistance Center");
            sb.AddNewHtmlLine();
            sb.Append("<a href='https://portal.shiptacenter.org'>portal.shiptacenter.org</a>");
            sb.AddNewHtmlLine();
            sb.Append(ConfigUtil.ShiptalkSupportPhone);
            sb.AddNewHtmlLines(5);

            return sb.ToString();
        }

        private bool SendEmailToUserAboutPasswordChange()
        {
            //string EmailAddress = UserName; commented by BM, user not yest logged in so, account info does not have e-mail.
            // We will get e-mail from User account

            // ShiptalkMailMessage mailMessage = new ShiptalkMailMessage(true, ShiptalkMailMessage.MailFrom.ShiptalkResourceCenter);
            // mailMessage.ToList.Add(EmailAddress);

            //Prepare Mail Object
            ShiptalkMailMessage mailMessage = new ShiptalkMailMessage(true, ShiptalkMailMessage.MailFrom.ShiptalkResourceCenter);



            if (ConfigUtil.WebEnvironment != "prod")
            {
                mailMessage.Subject = "Changes to your account at shiptacenter.org(" + ConfigUtil.WebEnvironment + ")";

            }
            else
            {
                mailMessage.Subject = "Changes to your account at shiptacenter.org";
            }

            mailMessage.Body = CreateEmailBodyForPasswordChangeConfirmation();

            //Send Mail here
            ShiptalkMail mail = new ShiptalkMail(mailMessage);
            return mail.SendMail();
        }

        private string CreateEmailBodyForPasswordChangeConfirmation()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            if (ConfigUtil.WebEnvironment != "prod")
            {
                sb.AppendFormat("-----------TEST  Your Password Changed DEV site------------"); sb.AddNewHtmlLines(2);
                sb.AppendFormat("-----------Created from DEV environment: {0}------------", ConfigUtil.WebEnvironment); sb.AddNewHtmlLines(2); ;
            }
            sb.Append("Hello,");
            sb.AddNewHtmlLines(2);
            sb.Append("This email is to confirm that your password at shiptacenter.org has been changed successfully.");
            sb.AddNewHtmlLines(2);
            sb.Append("If you did not change the password, please contact SHIP National Technical Assistance Center immediately.");
            sb.AddNewHtmlLines(3);
            sb.Append("Thank you,");
            sb.AddNewHtmlLine();
            sb.Append("SHIP National Technical Assistance Center");
            sb.AddNewHtmlLine();
            sb.Append("<a href='portal.shiptacenter.org'>portal.shiptacenter.org</a>");
            sb.AddNewHtmlLine();
            sb.Append(ConfigUtil.ShiptalkSupportPhone);
            sb.AddNewHtmlLines(5);

            return sb.ToString();
        }


        public bool SendPasswordResetEmail(string userEmailId, string resetGUID)
        {
            //Prepare Mail Object
            ShiptalkMailMessage mailMessage = new ShiptalkMailMessage(true, ShiptalkMailMessage.MailFrom.ShiptalkResourceCenter);

            mailMessage.ToList.Add(userEmailId);

            if (ConfigUtil.WebEnvironment != "prod")
            {
                mailMessage.Subject = "Your request at the shiptacenter.org(" + ConfigUtil.WebEnvironment + ")";

            }
            else
            {
                mailMessage.Subject = "Your request at the shiptacenter.org";
            }

            mailMessage.Body = CreateEmailBodyForResetPassword(userEmailId, resetGUID);

            //Send Mail here
            ShiptalkMail mail = new ShiptalkMail(mailMessage);
            return mail.SendMail();

        }

        private string CreateEmailBodyForResetPassword(string userEmailId, string resetGUID)
        {



            string linkFormat = "<a href='" + ConfigUtil.PasswordResetUrl + "?email={0}&guid={1}'>Follow this link</a>";
            string ResetLink = string.Format(linkFormat, userEmailId, resetGUID);
            string textlink = ConfigUtil.PasswordResetUrl + "?email={0}&guid={1}";
            textlink = string.Format(textlink, userEmailId, resetGUID);

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            if (ConfigUtil.WebEnvironment != "prod")
            {
                sb.AppendFormat("-----------TEST Reset Your Password from DEV site------------"); sb.AddNewHtmlLines(2);
                sb.AppendFormat("-----------Created from DEV environment: {0}------------", ConfigUtil.WebEnvironment); sb.AddNewHtmlLines(2); ;
            }
            sb.Append("Hello,");
            sb.AddNewHtmlLines(2);
            sb.Append("A request to reset your password was made at <a href='www.shiptacenter.org'>www.shiptacenter.org.</a>");
            sb.AddNewHtmlLine();
            sb.Append("If you did not request your password reset, please disregard this message.");
            sb.AddNewHtmlLines(2);


            sb.Append(ResetLink);//Follow this link ahref
            sb.Append(" within 72 hours to reset your password. If you have difficulties accessing the link, copy and paste the link below into your browser’s address bar to verify your email address.");
            sb.AddNewHtmlLines(2);
            sb.Append(textlink);
            sb.AddNewHtmlLines(3);
            sb.Append("Note: If you are reading this email more than 72 hours after your request, you will need to generate a new request. Go to <a href='www.shiptacenter.org'>www.shiptacenter.org</a>, click the orange SHIP Login padlock, and click forgot your password.");

            sb.AddNewHtmlLines(3);
            sb.Append("Thank you,");
            sb.AddNewHtmlLine();
            sb.Append("SHIP National Technical Assistance Center.");
            sb.AddNewHtmlLine();
            sb.Append("<a href='www.shiptacenter.org'>www.shiptacenter.org</a>");
            sb.AddNewHtmlLine();
            sb.Append(ConfigUtil.ShiptalkSupportPhone);
            sb.AddNewHtmlLines(5);

            return sb.ToString();
        }

        private bool SendPasswordCanNotBeChangedEmail()
        {
            //Prepare Mail Object
            ShiptalkMailMessage mailMessage = new ShiptalkMailMessage(true, ShiptalkMailMessage.MailFrom.ShiptalkResourceCenter);

            // mailMessage.ToList.Add(EmailAddress);
            mailMessage.Subject = "Your request at the shiptacenter.org";

            mailMessage.Body = CreateEmailBodyForPasswordCanNotBeChanged();

            //Send Mail here
            ShiptalkMail mail = new ShiptalkMail(mailMessage);
            return mail.SendMail();
        }

        private string CreateEmailBodyForPasswordCanNotBeChanged()
        {


            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("You already changed your password. You are allowed to change your password once in a day");
            sb.AddNewHtmlLines(2);
            sb.Append("Contact help desk to unlock, verify your identity, and change password.");

            sb.AddNewHtmlLines(3);
            sb.Append("Thank you,");
            sb.AddNewHtmlLine();
            sb.Append("SHIPNPR Help Desk");
            sb.AddNewHtmlLine();
            sb.Append("<a href='portal.shiptacenter.org'>portal.shiptacenter.org</a>");
            sb.AddNewHtmlLine();
            sb.Append(ConfigUtil.ShiptalkSupportPhone);
            sb.AddNewHtmlLines(5);

            return sb.ToString();
        }


        public static bool SendEmailNOTUSED(MailMessage message)
        {

            SmtpClient smtp = new SmtpClient();
            smtp.EnableSsl = true;
            smtp.Host = ConfigurationManager.AppSettings["SmtpHost"];
            smtp.Port = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(
                ConfigurationManager.AppSettings["SmtpCredentialsUid"],
                ConfigurationManager.AppSettings["SmtpCredentialsPwd"]);

            message.From = new MailAddress(ConfigurationManager.AppSettings["From"]);

            message.Sender = new MailAddress(ConfigurationManager.AppSettings["From"]);


            try
            {
                smtp.Send(message);
                return true;

            }
            catch (SmtpException smtpEx)
            {
                // Log or manage your error here, then...
                //return RedirectToUmbracoPage(1063); // <- My published error page.
                return false;

            }

            catch (Exception ex)
            {
                //TODO : Logout

                return false;
            }
        }


        //Resource email related
        public bool SendResourceWaitingForApproval(string emailId, string firstName)
        {
            //Send a reset email to member
            // Create the email object first, then add the properties.
            //Prepare Mail Object
            ShiptalkMailMessage mailMessage = new ShiptalkMailMessage(true, ShiptalkMailMessage.MailFrom.ShiptalkResourceCenter);

            //Send to the member's email address
            mailMessage.ToList.Add(emailId);

            //Subject
            if (ConfigUtil.WebEnvironment != "prod")
            {
                mailMessage.Subject = "Your shiptacenter.org resource creation(" + ConfigUtil.WebEnvironment + ")";

            }
            else
            {
                mailMessage.Subject = "Your shiptacenter.org resource creation";
            }


            mailMessage.Body = CreateResourceWaitingForApprovalEmailBody(firstName);

            //Send Mail here
            ShiptalkMail mail = new ShiptalkMail(mailMessage);
            return mail.SendMail();
        }
        private static string CreateResourceWaitingForApprovalEmailBody(string firstName)
        {



            //HTML Message
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (ConfigUtil.WebEnvironment != "prod")
            {
                sb.AppendFormat("-----------TEST  Resource Creation DEV site------------"); sb.AddNewHtmlLines(2);
                sb.AppendFormat("-----------Created from DEV environment: {0}------------", ConfigUtil.WebEnvironment); sb.AddNewHtmlLines(2); ;
            }
            sb.AppendFormat("Dear {0},", firstName.ToCamelCasing());
            sb.AddNewHtmlLines(2);
            sb.Append("Your resource has been uploaded and is awaiting approval from your SHIP director or SHIP administrator.");
            sb.AddNewHtmlLines(3);

            sb.AddNewHtmlLines(3);
            sb.Append(" You will receive an email from info@shiptacenter.org after it has been reviewed, notifying you of the status.");
            sb.AddNewHtmlLines(3);
            sb.Append("Sincerely,");
            sb.AddNewHtmlLines(2);
            sb.Append("The SHIP National Technical Assistance Center");
            sb.AddNewHtmlLine();
            sb.Append(ConfigUtil.ShiptalkSupportPhone);
            sb.AddNewHtmlLine();
            sb.Append(ConfigUtil.EmailOfResourceCenter);
            sb.AddNewHtmlLines(5);

            return sb.ToString();

        }


        public bool SendDesignatedNotificationForResourceApproval(string state, string firstName, string lastName)
        {

            // var mailMessage = new MailMessage();

            //Prepare Mail Object
            ShiptalkMailMessage mailMessage = new ShiptalkMailMessage(true, ShiptalkMailMessage.MailFrom.ShiptalkResourceCenter);


            int intStateAdmin = (int)UiEnum.ShipRolesUmbraco.shipadmin;

            int intStateDir = (int)UiEnum.ShipRolesUmbraco.shipdirector;


            var userStateAdminList = ApplicationContext.Current.Services.MemberService.GetMembersByPropertyValue("RoleAssigned", intStateAdmin, ValuePropertyMatchType.Exact).Where(x => x.IsApproved);
            var userStateDirList = ApplicationContext.Current.Services.MemberService.GetMembersByPropertyValue("RoleAssigned", intStateDir, ValuePropertyMatchType.Exact).Where(x => x.IsApproved);
            var memberAdmin = userStateAdminList.Where(x => x.Properties["state"].Value.ToString() == state && x.Properties["isInactive"].Value.ToString() != "1");
            var membersDir = userStateDirList.Where(x => x.Properties["state"].Value.ToString() == state && x.Properties["isInactive"].Value.ToString() != "1");


            foreach (var member in memberAdmin)
            {

                mailMessage.ToList.Add(member.Email);

            }

            foreach (var member in membersDir)
            {

                mailMessage.ToList.Add(member.Email);

            }

            if (ConfigUtil.WebEnvironment != "prod")
            {
                mailMessage.Subject = "New resource uploaded at shiptacenter.org(" + ConfigUtil.WebEnvironment + ")";

            }
            else
            {
                mailMessage.Subject = "New resource uploaded at shiptacenter.org";
            }
            mailMessage.Body = CreateEmailBodyForResourceNotification(firstName,lastName);


            //Send Mail here
            ShiptalkMail mail = new ShiptalkMail(mailMessage);
            return mail.SendMail();


        }
        private static string CreateEmailBodyForResourceNotification(string firstName, string lastName)
        {


            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (ConfigUtil.WebEnvironment != "prod")
            {
                sb.AppendFormat("-----------TEST Resource request for approval------------"); sb.AddNewHtmlLines(2);
                sb.AppendFormat("-----------Created from DEV environment: {0}------------", ConfigUtil.WebEnvironment); sb.AddNewHtmlLines(2);
            }
            sb.AppendFormat("A resource from {0} {1} at your SHIP has been uploaded for possible inclusion in the SHIP Resource Library at portal.shiptacenter.org.", firstName.ToCamelCasing(), lastName.ToCamelCasing());
            sb.AddNewHtmlLine();
            sb.Append("As a reminder, uploads from staff-level users of the SHIP Resource Library do not go live until they are approved by you or another SHIP administrator in your state, territory, or commonwealth.");

            sb.AddNewHtmlLines(3);


            sb.Append("Please visit your administrator dashboard at portal.shiptacenter.org to review and approve (or delete) the resource. If you do not find this resource on your dashboard when you visit, it means another SHIP administrator from your program had already reviewed and approved or rejected the resource.");
            sb.AddNewHtmlLines(2);



            //Need to add User Descriptor and Regional Access profile by verifying what role or agency was selected.

            sb.Append("Sincerely,");
            sb.AddNewHtmlLines(2);
            sb.Append("The SHIP National Technical Assistance Center");
            sb.AddNewHtmlLine();
            sb.Append(ConfigUtil.ShiptalkSupportPhone);
            sb.AddNewHtmlLine();
            sb.Append(ConfigUtil.EmailOfResourceCenter);
            sb.AddNewHtmlLines(5);


            return sb.ToString();
        }


        public bool SendResourceApproval(string emailId, string fullName)
        {
            //Send a reset email to member
            // Create the email object first, then add the properties.
            //Prepare Mail Object
            ShiptalkMailMessage mailMessage = new ShiptalkMailMessage(true, ShiptalkMailMessage.MailFrom.ShiptalkResourceCenter);



            //Send to the member's email address
            mailMessage.ToList.Add(emailId);

            //Subject
            if (ConfigUtil.WebEnvironment != "prod")
            {
                mailMessage.Subject = "Your shiptacenter.org resource approval(" + ConfigUtil.WebEnvironment + ")";

            }
            else
            {
                mailMessage.Subject = "Your shiptacenter.org resource approval";
            }




            mailMessage.Body = CreateResourceForApprovalEmailBody(fullName);

            //Send Mail here
            ShiptalkMail mail = new ShiptalkMail(mailMessage);
            return mail.SendMail();
        }
        private static string CreateResourceForApprovalEmailBody(string fullName)
        {



            //HTML Message
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (ConfigUtil.WebEnvironment != "prod")
            {
                sb.AppendFormat("-----------TEST  Resource approval DEV site------------"); sb.AddNewHtmlLines(2);
                sb.AppendFormat("-----------Created from DEV environment: {0}------------", ConfigUtil.WebEnvironment); sb.AddNewHtmlLines(2); ;
            }
            sb.AppendFormat("Dear {0},", fullName.ToCamelCasing());
            sb.AddNewHtmlLines(2);
            sb.Append("The resource you uploaded for the SHIP Resource Library at portal.shiptacenter.org has now been approved for publication by a SHIP director or SHIP administrator.");

            sb.AddNewHtmlLines(3);
            sb.Append("To view your resource, log in at portal.shiptacenter.org.");
            sb.AddNewHtmlLines(3);
            sb.Append("Sincerely,");
            sb.AddNewHtmlLines(2);
            sb.Append("The SHIP National Technical Assistance Center");
            sb.AddNewHtmlLine();
            sb.Append(ConfigUtil.ShiptalkSupportPhone);
            sb.AddNewHtmlLine();
            sb.Append(ConfigUtil.EmailOfResourceCenter);
            sb.AddNewHtmlLines(5);

            return sb.ToString();

        }

        public bool SendResourceDenial(string emailId, string fullName)
        {
            //Send a reset email to member
            // Create the email object first, then add the properties.
            //Prepare Mail Object
            ShiptalkMailMessage mailMessage = new ShiptalkMailMessage(true, ShiptalkMailMessage.MailFrom.ShiptalkResourceCenter);



            //Send to the member's email address
            mailMessage.ToList.Add(emailId);

            //Subject
            if (ConfigUtil.WebEnvironment != "prod")
            {
                mailMessage.Subject = "Your shiptacenter.org upload was not approved(" + ConfigUtil.WebEnvironment + ")";

            }
            else
            {
                mailMessage.Subject = "Your shiptacenter.org upload was not approved";
            }




            mailMessage.Body = CreateResourceForDenaialEmailBody(fullName);

            //Send Mail here
            ShiptalkMail mail = new ShiptalkMail(mailMessage);
            return mail.SendMail();
        }
        private static string CreateResourceForDenaialEmailBody(string fullName)
        {



            //HTML Message
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (ConfigUtil.WebEnvironment != "prod")
            {
                sb.AppendFormat("-----------TEST  Resource denial DEV site------------"); sb.AddNewHtmlLines(2);
                sb.AppendFormat("-----------Created from DEV environment: {0}------------", ConfigUtil.WebEnvironment); sb.AddNewHtmlLines(2); ;
            }
            sb.AppendFormat("Dear {0},", fullName.ToCamelCasing());
            sb.AddNewHtmlLines(2);
            sb.Append("The resource you uploaded for the SHIP Resource Library at portal.shiptacenter.org was not approved for publication by a SHIP director or SHIP administrator.");

            sb.AddNewHtmlLines(3);
            sb.Append("If you have questions about this decision, please contact a SHIP director or administrator from your SHIP program.");
            sb.AddNewHtmlLines(3);
            sb.Append("Sincerely,");
            sb.AddNewHtmlLines(2);
            sb.Append("The SHIP National Technical Assistance Center");
            sb.AddNewHtmlLine();
            sb.Append(ConfigUtil.ShiptalkSupportPhone);
            sb.AddNewHtmlLine();
            sb.Append(ConfigUtil.EmailOfResourceCenter);
            sb.AddNewHtmlLines(5);

            return sb.ToString();

        }
    }
}
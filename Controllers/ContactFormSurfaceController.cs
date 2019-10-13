using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;
using UmbracoShipTac.Models;
using umbraco.NodeFactory;
using umbraco;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Configuration;


namespace UmbracoShipTac.Controllers
{
    public class ContactFormSurfaceController : SurfaceController
    {
        //
        // GET: /ContactFormSurface/

        public ActionResult Index()
        {

            var model = new ContactFormViewModel();
            return PartialView("ContactForm", model);
        }

        [HttpPost]
        public ActionResult HandleFormSubmit(ContactFormViewModel model )
        {

            if (ModelState.IsValid)
            {
                var sb = new StringBuilder();

                sb.AppendFormat("<p>Email from website: {0}</p>", model.Title);

                sb.AppendFormat("<p>FName: {0}</p>", model.FName);
                sb.AppendFormat("<p>LName: {0}</p>", model.LName);

                sb.AppendFormat("<p>Email: {0}</p>", model.Email);
                sb.AppendFormat("<p>Title: {0}</p>", model.Title);
                sb.AppendFormat("<p>Address1: {0}</p>", model.Address1);
                sb.AppendFormat("<p>Address2: {0}</p>", model.Address2);
                sb.AppendFormat("<p>City: {0}</p>", model.City);
                sb.AppendFormat("<p>Origin: {0}</p>", model.Origin);
                sb.AppendFormat("<p>Zip: {0}</p>", model.Zip);
 

                sb.AppendFormat("<p>Phone: {0}</p>", model.Phone);

                sb.AppendFormat("<p>Comments: {0}</p>", model.Comments);

                SmtpClient smtp = new SmtpClient();
                smtp.EnableSsl = true;
                smtp.Host = ConfigurationManager.AppSettings["SmtpHost"];
                smtp.Port =int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(
                    ConfigurationManager.AppSettings["SmtpCredentialsUid"],
                    ConfigurationManager.AppSettings["SmtpCredentialsPwd"]);


                MailMessage message = new MailMessage();

                message.Subject = "Inquiry";

                message.From = new MailAddress(
                    ConfigurationManager.AppSettings["From"]);


                string toAddress = ConfigurationManager.AppSettings["To"];
                if (toAddress.Contains(","))
                    message.To.Add(toAddress); //multiple address found
                else
                    message.To.Add(new MailAddress(toAddress)); //only one address found
                
                message.Sender = new MailAddress(ConfigurationManager.AppSettings["From"]);
                message.Body = sb.ToString();
                message.IsBodyHtml = true;

                try
                {
                    smtp.Send(message);

                }
                catch (SmtpException smtpEx)
                {
                    // Log or manage your error here, then...
                    return RedirectToUmbracoPage(1063); // <- My published error page.
                }

                TempData["success"] = true;
                return CurrentUmbracoPage();
            }
            else 
            {

                return CurrentUmbracoPage();
            }
        }
    }
}

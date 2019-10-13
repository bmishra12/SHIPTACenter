using System.Web;
using System.IO;
using System.Web.Configuration;
using System.Web.Mvc;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace UmbracoShipTac.Controllers
{
    public class MediaController : Controller
    {
        public ActionResult IndexOriginal(string id, string file)
        {
            string mediaPath = "/media/" + id + "/" + file;

            IMediaService mediaService = ApplicationContext.Current.Services.MediaService;

            IMedia media = mediaService.GetMediaByPath(mediaPath);

            if (media != null)
            {
                bool requiresMemberLogin = media.GetValue<bool>("requiresMemberLogin");

                if (requiresMemberLogin == true)
                {
                    if (!User.Identity.IsAuthenticated)
                    {
                        System.Configuration.Configuration configuration = WebConfigurationManager.OpenWebConfiguration("/");
                        AuthenticationSection authenticationSection = (AuthenticationSection)configuration.GetSection("system.web/authentication");
                        FormsAuthenticationConfiguration formsAuthentication = authenticationSection.Forms;

                        string redirectUrl = formsAuthentication.LoginUrl + "?ReturnUrl=" + Url.Encode(mediaPath);
                        return Redirect(redirectUrl);
                    }
                }

                FileStream fileStream = new FileStream(Server.MapPath(mediaPath), FileMode.Open);

                return new FileStreamResult(fileStream, MimeMapping.GetMimeMapping(file));
            }
            else
            {
                return HttpNotFound();
            }
        }


        public ActionResult Index(string id, string file)
        {
            string mediaPath = "/media/" + id + "/" + file;

            IMediaService mediaService = ApplicationContext.Current.Services.MediaService;

            IMedia media = mediaService.GetMediaByPath(mediaPath);

            if (media != null)
            {
               // bool requiresMemberLogin = media.GetValue<bool>("requiresMemberLogin");

                // Make sure it's an image.
                //Check if we're dealing with a pdf document
                if (!(Path.GetExtension(file) == ".png"))
                {
        
 
                    if (!User.Identity.IsAuthenticated)
                    {
                        return HttpNotFound();
                    }
                }
                FileStream fileStream = new FileStream(Server.MapPath(mediaPath), FileMode.Open);
                return new FileStreamResult(fileStream, MimeMapping.GetMimeMapping(file));
            }
            else
            {
                return HttpNotFound();
            }
        }
    }
}
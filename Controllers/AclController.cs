
using System.Web.Mvc;

using System.Linq;

using Umbraco.Core.Services;

using Umbraco.Web.Models;
using UmbracoShipTac.Models;
using System.Collections.Generic;

namespace UmbracoShipTac.Controllers
{
    public class TextPagController : Umbraco.Web.Mvc.RenderMvcController
    {
       ////// public override ActionResult Index(RenderModel model)

        public ActionResult ProductTyreSelectorViewModel(ProductTyreSelectorViewModel model)
        {
            //Do some stuff here, then return the base method
           ////// return base.Index(model);


           ////// ProductTyreSelectorViewModel productTyreSelectorViewModel = new ProductTyreSelectorViewModel(model);

            var state = Request.QueryString["State"];
            var years = Request.QueryString["Years"];
            var models = Request.QueryString["Models"];


          

            var iMembers = Services.MemberService.GetMembersByPropertyValue("State", state).Where(x => !x.IsApproved).OrderByDescending(x => x.Name);

            List<UserView> users = new List<UserView>();

            foreach (Umbraco.Core.Models.IMember amember in iMembers)
            {
            users.Add( new UserView{
                Name = amember.Name, 
                Email= amember.Email,
                  Status="2009"} );
            }

            if (Request.QueryString.Count == 0)
            {
               //// return CurrentTemplate(productTyreSelectorViewModel);
                return CurrentTemplate(model);
            }


           //// productTyreSelectorViewModel.Tyres = users;

            model.UsersView = users;


            var template = ControllerContext.RouteData.Values["action"].ToString();

            //return an empty content result if the template doesn't physically 
            //exist on the file system
            if (!EnsurePhsyicalViewExists(template))
            {
                return Content("Could not find physical view template.");
            }

            return CurrentTemplate(model);
        }
    }
}
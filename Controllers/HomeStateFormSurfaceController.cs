using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;
using UmbracoShipTac.Models;
using umbraco.NodeFactory;
using umbraco;


namespace UmbracoShipTac.Controllers
{
    public class HomeStateFormSurfaceController : SurfaceController
    {
        //
        // GET: /ContactFormSurface/

        public ActionResult Index()
        {

            StateFormViewModel model = new StateFormViewModel();


            int stateRootID = UmbracoShipTac.Code.ConfigUtil.AllStateRootId;

            model.ListName = Services.ContentService.GetChildren(stateRootID).Select(x => new SelectListItem()
             {
                 Text = x.GetValue("dropdowntext").ToString(),
                 Value = x.Id.ToString()
             });


            return PartialView("HomeStateForm", model);

            //return CurrentTemplate
        }

        [HttpPost]
        public ActionResult HandleFormSubmit(StateFormViewModel model)
        {

            if (ModelState.IsValid)
            {
                IEnumerable<Node> nodes = uQuery.GetNodesByName("item");

                //Response.Redirect(umbraco.library.NiceUrl(nodeId), false);


                ///return RedirectToCurrentUmbracoPage(model.Name);
                int idm = model.PageId;

                return RedirectToUmbracoPage(idm);
            }

            //redirec to current page
            return RedirectToCurrentUmbracoPage();
        }
    }
}

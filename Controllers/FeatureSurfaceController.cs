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
using System.Web.Script.Serialization;
using Examine;
using Examine.SearchCriteria;
using Examine.LuceneEngine.SearchCriteria;



namespace UmbracoShipTac.Controllers.SurfaceControllers
{


    public class FeatureSurfaceController : SurfaceController
    {



        public ActionResult RenderFeatureTopRecent(UserFeatureTopViewModel model)
        {

            model.TopListView = GetTopRecentFeatures(model);

            //check the role of the user.
           
            //if the user is  SHIP administrators and SHIP director then only count the pending 
            //for other role set it to zero so that the yellow round pending button wont show.


            if (System.Web.HttpContext.Current.User.IsInRole("shipdirector")
                || System.Web.HttpContext.Current.User.IsInRole("shipadmin"))
            {

                int pusers = CountPendingUsers();
                int presouces = CountPendingResources();
                model.PendingCount = pusers + presouces;
            }
            else
            {
                model.PendingCount = 0;
            }

            return PartialView("feature", model);
        }

        private List<SResult> GetTopRecentFeatures(UserFeatureTopViewModel model)
        {
            int featureRootId = 0;

            List<SResult> features = new List<SResult>();

            featureRootId = UmbracoShipTac.Code.ConfigUtil.FeatureRootId;

            string[] roles = System.Web.Security.Roles.GetRolesForUser(User.Identity.Name);
          //  filter = filter.And().Field("roles", roles[0]);


            //IEnumerable<Umbraco.Core.Models.IContent> items;
            //items = Services.ContentService.GetDescendants(featureRootId).Where(x => x.ContentType.Alias == "UserFeature"
            //    && x.GetValue("roles").ToString().Contains(roles[0]))
            //    .OrderByDescending(x => x.CreateDate).Take(8);


            //sort by updatedate rather than created date..
            IEnumerable<Umbraco.Core.Models.IContent> items;
            items = Services.ContentService.GetDescendants(featureRootId).Where(x => x.ContentType.Alias == "UserFeature"
                && myfunc(x.GetValue("role").ToString())
                 .Contains(roles[0]))
                .OrderByDescending(x => x.UpdateDate).Take(8);



            //items = Services.ContentService.GetDescendants(featureRootId);


               
        //if (CurrentPage.HasValue("featureImage")){                                         
        //var dynamicMediaItem = Umbraco.Media(CurrentPage.featureImage);
        //<img src="@dynamicMediaItem.umbracoFile" alt="@dynamicMediaItem.Name"/>

        dynamic dynamicMediaItem ;
        string imgurl = null;
            foreach (var aItem in items)
            {

                if (aItem.GetValue("featureimage") != null)
                {
                    dynamicMediaItem = Umbraco.Media(aItem.GetValue("featureimage"));
                    imgurl = dynamicMediaItem.umbracoFile;
                }
                else
                    imgurl = "";


                

                SResult result = new SResult()
                {
                   

                    Id = aItem.Id.ToString(),
                    Url = umbraco.library.NiceUrl(Convert.ToInt32(aItem.Id.ToString())),
                    NodeName = aItem.GetValue("featureheader").ToString(), //aItem.Name, FeatureHeader
                   // Text = "Test this one"
                    Text = System.Text.RegularExpressions.Regex.Replace(aItem.GetValue("featurecontent").ToString(), "<.*?>", String.Empty),
                    ImageUrl = imgurl
                };

                //do the difference of nodename count 
                int charToTake = 200 - result.NodeName.Length;

                result.Text = result.Text.Substring(0, Math.Min(result.Text.Length, charToTake));

                features.Add(result);
            }

     
            return features;
        }

        private string myfunc(string uroles)
        {
            string result =  string.Empty;  //SHIP staff and higher  SHIP administrators and higher  SHIP directors and higher

            string dropdownvalue = umbraco.library.GetPreValueAsString(Convert.ToInt32(uroles)).Trim().ToLower();

              if (dropdownvalue == "ship counselors and higher") result = "shipcenter,acladmin,partner,shipdirector,shipadmin,shipstaff,shipcounselor";
              if (dropdownvalue == "ship staff and higher") result = "shipcenter,acladmin,partner,shipdirector,shipadmin,shipstaff";
             if (dropdownvalue  == "ship administrators and higher") result = "shipcenter,acladmin,partner,shipdirector,shipadmin";
              if (dropdownvalue == "ship directors and higher") result = "shipcenter,acladmin,partner,shipdirector";
    
            
            return result;
        }


        private int CountPendingUsers()
        {


            bool hasloggedin = (System.Web.HttpContext.Current.User != null) &&
                System.Web.HttpContext.Current.User.Identity.IsAuthenticated;

            var userService = Services.MemberService;
            string usern = string.Empty;
            usern = System.Web.HttpContext.Current.User.Identity.Name;


            //UmbracoShipTac.Code.UiEnum.RolesId.shipcounselor

            var amember = userService.GetByEmail(usern);

            //get the rolid of the  user.. from the isinrole
            string strRoleAssigned = amember.GetValue("roleAssigned").ToString();

            string state = string.Empty;
            state = amember.GetValue("state").ToString();

           

            // Use the already configured member searcher
            var memberSearcher = ExamineManager.Instance
                .SearchProviderCollection["InternalMemberSearcher"]
                   .CreateSearchCriteria(BooleanOperation.Or);

            Examine.SearchCriteria.IBooleanOperation filter = null;
            filter = memberSearcher.Field("hasVerifiedEmail", "0"); //just to start a dummy this is OR ? I think It is AND
            filter = filter.And().Field("umbracoMemberApproved", "0");



            if (state.ToUpper() != "ALL")
            {
                filter = filter.And().Field("state", state);
            }


            filter = filter.And().Range("roleAppliedFor", "1", strRoleAssigned, true, true);

            filter = filter.And().Field("isDenied", "0");

            ISearchResults resultsAllMembers = ExamineManager.Instance
                .SearchProviderCollection["InternalMemberSearcher"]
                .Search(filter.Compile());


            int pendingusers = resultsAllMembers.Count();

           //do not include the loggedin user..
          // if (id != amember.Id.ToString())

            return pendingusers;
  
        }


        private int CountPendingResources()
        {

            bool hasloggedin = (System.Web.HttpContext.Current.User != null) &&
                System.Web.HttpContext.Current.User.Identity.IsAuthenticated;

            var userService = Services.MemberService;
            string usern = string.Empty;
            usern = System.Web.HttpContext.Current.User.Identity.Name;


            //UmbracoShipTac.Code.UiEnum.RolesId.shipcounselor

            var amember = userService.GetByEmail(usern);

            //get the rolid of the  user.. from the isinrole
            string strRoleAssigned = amember.GetValue("roleAssigned").ToString();

            string state = string.Empty;
            state = amember.GetValue("state").ToString();

            int unpublishId = UmbracoShipTac.Code.ConfigUtil.ResourceUnpublishedId;

            IEnumerable<Umbraco.Core.Models.IContent> items;
            if (state == "ALL")
            {
                //items = userList.OrderBy(x => x.Properties["state"].Value.ToString());
                items = Services.ContentService.GetChildren(unpublishId).OrderByDescending(x => x.Properties["state"].Value.ToString()).ThenByDescending(x => x.CreateDate);

            }
            else
            {
                items = Services.ContentService.GetChildren(unpublishId).Where(x => x.Properties["state"].Value.ToString() == state).OrderByDescending(x => x.CreateDate);

            }


            //var itemse = userService.GetMembersByPropertyValue("State", state).Where(x => !x.IsApproved).OrderByDescending(x => x.Name);

            int pendingresources = items.Count();

            //do not include the loggedin user..
            // if (id != amember.Id.ToString())

            return pendingresources;
        }
    }
}

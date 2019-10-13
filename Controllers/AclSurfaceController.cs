using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;
using UmbracoShipTac.Models;
using Umbraco.Core;
using Umbraco.Core.Persistence.Querying;
using UmbracoShipTac.Code;
using Examine;
using Examine.SearchCriteria;
using Examine.Providers;
using Examine.LuceneEngine.SearchCriteria;




namespace UmbracoShipTac.Controllers
{
    public class AclSurfaceController : SurfaceController
    {
       // Lucene.Net.Analysis.Standard.StandardAnalyzer.STOP_WORDS_SET ;

        /// <summary>
        /// Renders the Register View
        /// @Html.Action("RenderListUser","AprroveSurface");
        /// </summary>
        /// <returns></returns>
        /// called when link is pressed like prev and next page
        public ActionResult RenderAcl(PendingUserViewModel model)
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

            model.UsersView = searchPendingUsers(model);

            return PartialView("approvesurface/userpendinglist", model);
        }

        public ActionResult RenderApproved(UserViewModel model)
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

            model.UsersView = searchApprovedUsers(model);

            return PartialView("approvesurface/userapprovedlist", model);
        }


        public ActionResult RenderSearch(SearchViewModel model)
        {

            model.SerchsView = SearchTermForWeb(model.Term);

            model.SerchResourcView =  SearchTermForResources(model.Term);

            model.SerchEventView = SearchTermForEvents(model.Term);


            return PartialView("approvesurface/searchlist", model);
        }


        private List<UserView> searchPendingUsers(PendingUserViewModel model)
        {


            List<UserView> users = new List<UserView>();

            List<UserView> sortedList = new List<UserView>();

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

            if (!String.IsNullOrWhiteSpace(model.State))
            {
                state = model.State;
            }
            else
            {
                state = amember.GetValue("state").ToString();
                //set the modele state with correcte value of state.
                model.State = state;

            }


            //all OR then and ..to work in lucene index
            if (state.ToUpper() == "OR" || state.ToUpper() == "IN")
            {
                //do the old way of searching..
              ////  return searchPendingUsersOLD(model);

            }


            // Use the already configured member searcher
            var memberSearcher = ExamineManager.Instance
                .SearchProviderCollection["InternalMemberSearcher"]
                   .CreateSearchCriteria(BooleanOperation.Or);

            Examine.SearchCriteria.IBooleanOperation filter = null;
            filter = memberSearcher.Field("hasVerifiedEmail", "0"); //just to start a dummy this is OR ? I think It is AND
            filter = filter.And().Field("umbracoMemberApproved", "0");

          
         

            //if the name is supplied filter by name
            if (!model.NameSearch.IsNullOrWhiteSpace())
            {
                //filter = filter.And().Field("firstName", model.NameSearch);
                filter = filter.And().GroupedOr(new string[] { "firstName", "lastName" }, model.NameSearch);

            }

            if (state.ToUpper() != "ALL")
            {
                filter = filter.And().Field("state", state);
            }

     
            filter = filter.And().Range("roleAppliedFor", "1", strRoleAssigned, true, true);

            filter = filter.And().Field("isDenied", "0");

            ISearchResults resultsAllMembers = ExamineManager.Instance
                .SearchProviderCollection["InternalMemberSearcher"]
                .Search(filter.Compile());

            // Execute the query and get back search results
            // ISearchResults resultsAllMembers = memberSearcher.Search(allMembersCriteria);

            // Iterate through the results
            // Fields is a dictionary where the key is the property type alias and the value is a string
            foreach (var member in resultsAllMembers)
            {
                var fname = member.Fields.ContainsKey("firstName") ? member.Fields["firstName"] : "";
                //member.Fields["firstName"];
                var lname = member.Fields.ContainsKey("lastName") ? member.Fields["lastName"] : "";
                //member.Fields["lastName"];
                var id = member.Fields["id"];

                var appDate = member.Fields.ContainsKey("dateApproved") ? member.Fields["dateApproved"] : "";

                var denDate = member.Fields.ContainsKey("dateDenied") ? member.Fields["dateDenied"] : "";


                var updateDate = member.Fields.ContainsKey("updateDate") ? member.Fields["updateDate"] : "";


                var state1 = member.Fields.ContainsKey("state") ? member.Fields["state"] : "";

                var email = member.Fields.ContainsKey("email") ? member.Fields["email"] : "";



                string roleValue = member.Fields.ContainsKey("roleAssigned") ? member.Fields["roleAssigned"] : "";

                string isDenied = member.Fields.ContainsKey("isDenied") ? member.Fields["isDenied"] : "";
                string isInactive = member.Fields.ContainsKey("isInactive") ? member.Fields["isInactive"] : "";
                string hasVerifiedEmail = member.Fields.ContainsKey("hasVerifiedEmail") ? member.Fields["hasVerifiedEmail"] : "";
                string IsApproved = member.Fields.ContainsKey("umbracoMemberApproved") ? member.Fields["umbracoMemberApproved"] : "";

                //do not include the loggedin user..
                if (id != amember.Id.ToString())
                {

                    users.Add(
                           new UserView
                           {
                               ID = id,
                               Name = fname + " " + lname,
                               Email = email,
                               CreatedDate = TryMyParse(updateDate),
                               
                               Status = Utils.GetUserStatus(isDenied, isInactive, hasVerifiedEmail, IsApproved),
                               State = state1
                                //Status = auser.GetValue("hasVerifiedEmail").ToString() == "0" ? "Waiting for Email Verification" : "Waiting for Approval",

                           }
                           );
                }

            }

            //sort it by LastLoginDate this is the default..
            sortedList = users.OrderBy(o => o.LastLoginDate).ToList();

            return sortedList;
        }

        private List<UserView> searchPendingUsersOLD(PendingUserViewModel model)
        {
            List<UserView> users = new List<UserView>();

            bool hasloggedin = (System.Web.HttpContext.Current.User != null) &&
                System.Web.HttpContext.Current.User.Identity.IsAuthenticated;

            var userService = Services.MemberService;
            string usern = string.Empty;
            usern = System.Web.HttpContext.Current.User.Identity.Name;


            //UmbracoShipTac.Code.UiEnum.RolesId.shipcounselor

            var amember = userService.GetByEmail(usern);

            //get the rolid of the  user..
            int intRoleAssigned = int.Parse(amember.GetValue("roleAssigned").ToString());

            string state = string.Empty; //// 
            //request passes the state in the model.state

            if (!String.IsNullOrWhiteSpace(model.State))
            {
                state = model.State;
            }
            else
            {
                state = amember.GetValue("state").ToString();

                //set the modele state with correcte value of state.
                model.State = state;
            }




            // var usrs = userService.GetMembersByPropertyValue("State", state);

            //@if( @Roles.IsUserInRole("StateAdmin"))
            var pageSize = 8;
            var page = 1; int.TryParse(Request.QueryString["page"], out page);
            //var items = Model.Content.Children().Where(x => x.IsDocumentType("textpage")).OrderByDescending(x => x.CreateDate);
            //var date = DateTime.UtcNow.AddMinutes(-AppConstants.TimeSpanInMinutesToShowMembers);
            // var members = ApplicationContext.Current.Services.MemberService.GetMembersByPropertyValue(AppConstants.PropMemberLastActiveDate, date, ValuePropertyMatchType.GreaterThan)
            //                 .Where(x => x.IsApproved && !x.IsLockedOut);



            ////intRoleAssigned = 4; //RoleId 
            var userList = userService.GetMembersByPropertyValue("RoleAppliedFor", intRoleAssigned, ValuePropertyMatchType.LessThanOrEqualTo).Where(x => !x.IsApproved);

            IEnumerable<Umbraco.Core.Models.IMember> items;
            if (state == "ALL")
            {
                items = userList.OrderBy(x => x.Properties["state"].Value.ToString());
            }
            else
            {
                items = userList.Where(x => x.Properties["state"].Value.ToString() == state);
            }


            //if the name is supplied filter by name
            if (!model.NameSearch.IsNullOrWhiteSpace())
            {
                items = items.Where(x => x.Name.ToLower().Contains(model.NameSearch.ToLower()));
            }


            //var itemse = userService.GetMembersByPropertyValue("State", state).Where(x => !x.IsApproved).OrderByDescending(x => x.Name);

            var totalPages = (int)Math.Ceiling((double)items.Count() / (double)pageSize);

            if (page > totalPages)
            {
                page = totalPages;
            }
            else if (page < 1)
            {
                page = 1;
            }
            foreach (var auser in items)
            {
                //do not include the loggedin user..
                if (auser.Id != amember.Id)
                {
                    if (auser.GetValue("isDenied").ToString() != "1")
                    {


                        users.Add(
                            new UserView
                            {
                                ID = auser.Id.ToString(),
                                //Name = auser.Name,
                                Name = auser.GetValue("firstName").ToString() + " " + auser.GetValue("lastName").ToString(),
                                Email = auser.Email,

                                CreatedDate = TryMyParse(auser.CreateDate.ToString()),
                                Status = auser.GetValue("hasVerifiedEmail").ToString() == "0" ? "Waiting for Email Verification" : "Waiting for Approval",
                                State = auser.GetValue("state").ToString()
                            }
                            );
                    }
                }
            }


            return users;
        }

        private List<UserView> searchApprovedUsers(UserViewModel model)
        {


            List<UserView> users = new List<UserView>();

            List<UserView> sortedList = new List<UserView>();

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

            if (!String.IsNullOrWhiteSpace(model.State))
            {
                state = model.State;
            }
            else
            {
                state = amember.GetValue("state").ToString();
                //set the modele state with correcte value of state.
                model.State = state;

            }


            //all OR then and ..to work in lucene index
            if (state.ToUpper() == "OR" || state.ToUpper() == "IN")
            {
                //add a blank"

                //model.State = " "          + state;
                //do the old way of searching..
               // return searchApprovedUsersOLD(model);

            }

 
            // Use the already configured member searcher
            var  memberSearcher = ExamineManager.Instance
                .SearchProviderCollection["InternalMemberSearcher"]
                   .CreateSearchCriteria(BooleanOperation.Or);

            //did not work
          //  memberSearcher.SearchIndexType.RemoveStopWords();

            Examine.SearchCriteria.IBooleanOperation filter = null;
            filter = memberSearcher.Field("hasVerifiedEmail", "1"); //just to start a dummy this is OR

       


            //if the name is supplied filter by name
            if (!model.NameSearch.IsNullOrWhiteSpace())
            {
                //filter = filter.And().Field("firstName", model.NameSearch);
              //  filter = filter.And().GroupedOr(new string[] { "firstName", "lastName" }, model.NameSearch );


                //this makes a OR but giving OR results
                filter = filter.And().GroupedOr(new string[] { "firstName", "lastName" }, model.NameSearch.ToLower().MultipleCharacterWildcard());


            }

        
            filter = filter.And().Range("roleAppliedFor", "1", strRoleAssigned, true, true);
            filter = filter.And().Field("state", state);

            

           // Lucene.Net.Analysis.StopAnalyzer.ENGLISH_STOP_WORDS_SET = new System.Collections.Hashtable();

            ISearchResults resultsAllMembers = ExamineManager.Instance
                .SearchProviderCollection["InternalMemberSearcher"]
                .Search(filter.Compile());

            // Execute the query and get back search results
           // ISearchResults resultsAllMembers = memberSearcher.Search(allMembersCriteria);

            // Iterate through the results
            // Fields is a dictionary where the key is the property type alias and the value is a string
            foreach (var member in resultsAllMembers)
            {
                var fname = member.Fields.ContainsKey("firstName") ? member.Fields["firstName"] : "";
                    //member.Fields["firstName"];
                    var lname = member.Fields.ContainsKey("lastName") ? member.Fields["lastName"] : "";
                    //member.Fields["lastName"];
                var id = member.Fields["id"];

                var appDate = member.Fields.ContainsKey("dateApproved") ? member.Fields["dateApproved"] : "";

                var denDate = member.Fields.ContainsKey("dateDenied") ? member.Fields["dateDenied"] : "";
                

                var lastDate = member.Fields.ContainsKey("lastLoggedIn") ? member.Fields["lastLoggedIn"] : ""; 
  

               var state1 = member.Fields.ContainsKey("state") ? member.Fields["state"] : "";


                string roleValue = member.Fields.ContainsKey("roleAssigned") ? member.Fields["roleAssigned"] : "";

                string isDenied = member.Fields.ContainsKey("isDenied") ? member.Fields["isDenied"] : "";
                string isInactive = member.Fields.ContainsKey("isInactive") ? member.Fields["isInactive"] : "";
                string hasVerifiedEmail = member.Fields.ContainsKey("hasVerifiedEmail") ? member.Fields["hasVerifiedEmail"] : "";
                string IsApproved = member.Fields.ContainsKey("umbracoMemberApproved") ? member.Fields["umbracoMemberApproved"] : "";

                //do not include the loggedin user..
                if (id != amember.Id.ToString())
                {

                    users.Add(
                           new UserView
                           {
                               ID = id,
                               Role = GetUserRoleByValue(roleValue),
                               Name = fname + " " + lname,
                               ApprovedDate = appDate,
                               DeniedDate = denDate,
                               LastLoginDate = lastDate,
                               Status = Utils.GetUserStatus(isDenied, isInactive, hasVerifiedEmail, IsApproved)
                           }
                           );
                }

            }

            //sort it by LastLoginDate this is the default..
             sortedList = users.OrderBy(o => o.LastLoginDate).ToList();

            return sortedList;
        }


        private List<UserView> searchApprovedUsersOLD(UserViewModel model)
        {
            List<UserView> users = new List<UserView>();

            bool hasloggedin = (System.Web.HttpContext.Current.User != null) &&
                System.Web.HttpContext.Current.User.Identity.IsAuthenticated;

            var userService = Services.MemberService;
            string usern = string.Empty;
            usern = System.Web.HttpContext.Current.User.Identity.Name;


            //UmbracoShipTac.Code.UiEnum.RolesId.shipcounselor

            var amember = userService.GetByEmail(usern);

            //get the rolid of the  user..
            int intRoleAssigned = int.Parse(amember.GetValue("roleAssigned").ToString());

            string state = string.Empty;

            if (!String.IsNullOrWhiteSpace(model.State))
            {
                state = model.State;
            }
            else
            {
                state = amember.GetValue("state").ToString();
                //set the modele state with correcte value of state.
                model.State = state;

            }



            //var items = Model.Content.Children().Where(x => x.IsDocumentType("textpage")).OrderByDescending(x => x.CreateDate);
            //var date = DateTime.UtcNow.AddMinutes(-AppConstants.TimeSpanInMinutesToShowMembers);
            // var members = ApplicationContext.Current.Services.MemberService.GetMembersByPropertyValue(AppConstants.PropMemberLastActiveDate, date, ValuePropertyMatchType.GreaterThan)
            //                 .Where(x => x.IsApproved && !x.IsLockedOut);


            ////intRoleAssigned = 4; //RoleId 
            var userList = userService.GetMembersByPropertyValue("RoleAppliedFor", intRoleAssigned, ValuePropertyMatchType.LessThanOrEqualTo);

            var items = userList.Where(x => x.Properties["state"].Value.ToString() == state);

            //if the name is supplied filter by name
            if (!model.NameSearch.IsNullOrWhiteSpace())
            {
                items = items.Where(x => x.Name.ToLower().Contains(model.NameSearch.ToLower()));
            }




            foreach (var auser in items)
            {

                //do not include the loggedin user..
                if (auser.Id != amember.Id)
                {


                    users.Add(
                        new UserView
                        {
                            ID = auser.Id.ToString(),
                            Role = GetUserRole(auser),
                            //Name = auser.Name,
                            Name = auser.GetValue("firstName").ToString() + " " + auser.GetValue("lastName").ToString(),
                            ApprovedDate = TryMyParse(auser.GetValue("dateApproved").ToString()),
                            DeniedDate = TryMyParse(auser.GetValue("dateDenied").ToString()),
                            LastLoginDate = TryMyParse(auser.GetValue("lastLoggedIn").ToString()),
                            Status = Utils.GetUserStatus(auser),
                        }
                        );
                }
            }


            return users;
        }


        private List<SResult> SearchTermForWeb(string terms)
        {
 
            List<SResult> results = new List<SResult>();


            if (!string.IsNullOrEmpty(terms))
            {

               // var criteria = searcher.CreateSearchCriteria();
                //IBooleanOperation query = criteria.Field("contents", searchQuery.MultipleCharacterWildcard());


                // Find pages that contain our search text in either their nodeName or bodyText fields...
                // but exclude any pages that have been hidden.
                // searchCriteria.Fields("nodeName",terms.Boost(8)).Or().Field("metaTitle","hello".Boost(5)).Compile();

                Examine.SearchCriteria.ISearchCriteria crawl = null;
                Examine.SearchCriteria.ISearchCriteria crawlRestricted = null;

                if (User.Identity.IsAuthenticated)
                {
                    //this block of if is for the loggedin users...

                    string[] roles = System.Web.Security.Roles.GetRolesForUser(User.Identity.Name);

                    var criteriaRestricted = ExamineManager.Instance
                        .SearchProviderCollection["WebsiteSearcher"]
                        .CreateSearchCriteria();

                    //if there is space search for exact word.
                    IExamineValue srcTerm;
                    if (terms.Contains(" "))

                        srcTerm = terms.ToLower().Escape();
                    else
                        srcTerm = terms.ToLower().MultipleCharacterWildcard();

                    crawlRestricted = criteriaRestricted.GroupedOr(new string[] { "bodyText", "nodeName", "name", "menuline1", "menuline2", "title", "featurecontent", "seo" }, srcTerm)

                  .And().Field("GroupAccess", roles[0])
                        //.Or().Field("IsPublic", "true") //THE OR condition does not work as 
                                // when IsPublic =  ture group access is null
                                //so i have to combine both results..
                  .Not().Field("umbracoNaviHide", "1")
                  .Not().Field("nodeTypeAlias", "Image")
                  .Compile();

                    ISearchResults SearchResultsRestricted = ExamineManager.Instance
                    .SearchProviderCollection["WebsiteSearcher"]
                    .Search(crawlRestricted);

                    foreach (var sr in SearchResultsRestricted)
                    {
                        SResult result = new SResult()
                        {
                            Id = sr.Fields.ContainsKey("id") ? sr.Fields["id"] : "",
                            //Score = sr.Fields.ContainsKey("Score") ? (int)Math.Min(sr.Score * 100, 100) : 0,
                            // Url = sr.Fields.ContainsKey("urlName") ? sr.Fields["urlName"] : "",
                            //Url = sr.Fields.ContainsKey("Url") ? sr.Fields["Url"] : "",
                            Url = Umbraco.Content(sr.Fields["id"]).Url,
                            NodeName = sr.Fields.ContainsKey("nodeName") ? sr.Fields["nodeName"] : "",


                            Text = sr.Fields.ContainsKey("bodyText") ? sr.Fields["bodyText"] : "",

                            FText = sr.Fields.ContainsKey("featurecontent") ? sr.Fields["featurecontent"] : "",
                            Date = sr.Fields.ContainsKey("updateDate") ? sr.Fields["updateDate"] : "", 
                        };

                        result.Text = result.Text.Substring(0, Math.Min(result.Text.Length, 250));
                        result.FText = result.FText.Substring(0, Math.Min(result.FText.Length, 250));

                        results.Add(result);
                    }


                    var criteria = ExamineManager.Instance
                     .SearchProviderCollection["WebsiteSearcher"]
                     .CreateSearchCriteria();

                    crawl = criteria.GroupedOr(new string[] { "bodyText", "nodeName", "name", "menuline1", "menuline2", "title", "featurecontent", "seo" }, terms.ToLower().MultipleCharacterWildcard())

                    .Not().Field("umbracoNaviHide", "1")
                    .Not().Field("nodeTypeAlias", "Image")
                  .And().Field("IsPublic", "true")

                 .Compile();


                    ISearchResults SearchResults = ExamineManager.Instance
                     .SearchProviderCollection["WebsiteSearcher"]
                     .Search(crawl);


                    foreach (var sr in SearchResults)
                    {
                        SResult result = new SResult()
                        {
                            Id = sr.Fields.ContainsKey("id") ? sr.Fields["id"] : "",
                            Url = Umbraco.Content(sr.Fields["id"]).Url,
                            NodeName = sr.Fields.ContainsKey("nodeName") ? sr.Fields["nodeName"] : "",


                            Text = sr.Fields.ContainsKey("bodyText") ? sr.Fields["bodyText"] : "",

                            FText = sr.Fields.ContainsKey("featurecontent") ? sr.Fields["featurecontent"] : "",
                            Date = sr.Fields.ContainsKey("updateDate") ? sr.Fields["updateDate"] : "", 
                        };

                        result.Text = result.Text.Substring(0, Math.Min(result.Text.Length, 250));
                        result.FText = result.FText.Substring(0, Math.Min(result.FText.Length, 250));

                        results.Add(result);
                    }


                }
                else
                {
                    //this block for not authenticated...

                    var criteria = ExamineManager.Instance
                    .SearchProviderCollection["WebsiteSearcher"]
                    .CreateSearchCriteria();

                    //if there is space search for exact word.
                    IExamineValue srcTerm ;
                    if (terms.Contains(" ") )
                    
                        srcTerm = terms.ToLower().Escape();
                    else
                         srcTerm = terms.ToLower().MultipleCharacterWildcard();
                    

                    crawl = criteria.GroupedOr(new string[] { "bodyText", "nodeName", "name", "menuline1", "menuline2", "title", "featurecontent", "seo" }, srcTerm)

                        .Not().Field("umbracoNaviHide", "1")
                        .Not().Field("nodeTypeAlias", "Image")
                      .And().Field("IsPublic", "true")

                     .Compile();

                    ISearchResults SearchResults = ExamineManager.Instance
                  .SearchProviderCollection["WebsiteSearcher"]
                  .Search(crawl);


                    foreach (var sr in SearchResults)
                    {
                        SResult result = new SResult()
                        {
                            Id = sr.Fields.ContainsKey("id") ? sr.Fields["id"] : "",
                            //Score = sr.Fields.ContainsKey("Score") ? (int)Math.Min(sr.Score * 100, 100) : 0,
                            // Url = sr.Fields.ContainsKey("urlName") ? sr.Fields["urlName"] : "",
                            //Url = sr.Fields.ContainsKey("Url") ? sr.Fields["Url"] : "",
                            Url = Umbraco.Content(sr.Fields["id"]).Url,
                            NodeName = sr.Fields.ContainsKey("nodeName") ? sr.Fields["nodeName"] : "",


                            Text = sr.Fields.ContainsKey("bodyText") ? sr.Fields["bodyText"] : "",

                            FText = sr.Fields.ContainsKey("featurecontent") ? sr.Fields["featurecontent"] : "",
                            Date = sr.Fields.ContainsKey("updateDate") ? sr.Fields["updateDate"] : "", 
                        };

                        result.Text = result.Text.Substring(0, Math.Min(result.Text.Length, 250));
                        result.FText = result.FText.Substring(0, Math.Min(result.FText.Length, 250));

                        results.Add(result);
                    }

                }


            }

            //sort it:
           // if (results.Count>0)
            //    results = results.OrderBy(o => o.NodeName).ToList();

            //sort it by LastLoginDate this is the default..
            if (results.Count > 0)
                results = results.OrderByDescending(o => o.Date).ToList();


            return results;



        }

        private List<SResult> SearchTermForResources(string terms)
        {
            //the resource will be searched only for the logged in users

            List<SResult> results = new List<SResult>();


            if (!string.IsNullOrEmpty(terms))
            {


                // Find pages that contain our search text in either their nodeName or bodyText fields...
                // but exclude any pages that have been hidden.
                // searchCriteria.Fields("nodeName",terms.Boost(8)).Or().Field("metaTitle","hello".Boost(5)).Compile();

                Examine.SearchCriteria.ISearchCriteria crawlResourceRestricted = null;

                if (User.Identity.IsAuthenticated)
                {
                    string[] roles = System.Web.Security.Roles.GetRolesForUser(User.Identity.Name);

             
                    //Resouce search
                    var criteriaResourceRestricted = ExamineManager.Instance
                     .SearchProviderCollection["ResourceSearcher"]
                     .CreateSearchCriteria();


                    //if there is space search for exact word.
                    IExamineValue srcTerm;
                    if (terms.Contains(" "))

                        srcTerm = terms.ToLower().Escape();
                    else
                        srcTerm = terms.ToLower().MultipleCharacterWildcard();

                    crawlResourceRestricted = criteriaResourceRestricted.GroupedOr(new string[] { "description", "resoucetitle", "nodeName", "name", "seo", "resourcetypes", "subjects", "intendedaudiences", "activitys" }, srcTerm)

                  .And().Field("GroupAccess", roles[0])
                  .Not().Field("nodeTypeAlias", "Image")
                  .Compile();



                    ISearchResults SearchResultsResourceRestricted = ExamineManager.Instance
                    .SearchProviderCollection["ResourceSearcher"]
                    .Search(crawlResourceRestricted);


                    foreach (var sr in SearchResultsResourceRestricted)
                    {
                        SResult result = new SResult()
                        {
                            Id = sr.Fields.ContainsKey("id") ? sr.Fields["id"] : "",
                            Url = Umbraco.Content(sr.Fields["id"]).Url,
                            NodeName = sr.Fields.ContainsKey("nodeName") ? sr.Fields["nodeName"] : "",
                            Text = sr.Fields.ContainsKey("description") ? sr.Fields["description"] : "",
                            Date = sr.Fields.ContainsKey("updateDate") ? sr.Fields["updateDate"] : "", 
                        };

                        result.Text = result.Text.Substring(0, Math.Min(result.Text.Length, 250));
                        result.FText = "";
                        if (result.Text.Length > 249 && !result.Text.EndsWith("."))
                        {
                            result.FText = "1";

                        }
                        results.Add(result);
                    }

                }
            }

            //sort it by LastLoginDate this is the default..
            if (results.Count > 0)
                results = results.OrderByDescending(o => o.Date).ToList();


            return results;

        }


        private List<SResult> SearchTermForEvents(string terms)
        {
            //the resource will be searched only for the logged in users

            List<SResult> results = new List<SResult>();


            if (!string.IsNullOrEmpty(terms))
            {


                Examine.SearchCriteria.ISearchCriteria crawlResourceRestricted = null;

                if (User.Identity.IsAuthenticated)
                {
                    string[] roles = System.Web.Security.Roles.GetRolesForUser(User.Identity.Name);


                    //Resouce search
                    var criteriaResourceRestricted = ExamineManager.Instance
                     .SearchProviderCollection["EventSearcher"]
                     .CreateSearchCriteria();

                    //if there is space search for exact word.
                    IExamineValue srcTerm;
                    if (terms.Contains(" "))

                        srcTerm = terms.ToLower().Escape();
                    else
                        srcTerm = terms.ToLower().MultipleCharacterWildcard();

                    crawlResourceRestricted = criteriaResourceRestricted.GroupedOr(new string[] { "description", "eventtitle", "nodeName", "name", "seo", "intendedaudiences" }, srcTerm)

                  .And().Field("roles", roles[0])
                  .Compile();


                    ISearchResults SearchResultsResourceRestricted = ExamineManager.Instance
                    .SearchProviderCollection["EventSearcher"]
                    .Search(crawlResourceRestricted);


                    foreach (var sr in SearchResultsResourceRestricted)
                    {
                        SResult result = new SResult()
                        {
                            Id = sr.Fields.ContainsKey("id") ? sr.Fields["id"] : "",
                            Url = Umbraco.Content(sr.Fields["id"]).Url,
                            NodeName = sr.Fields.ContainsKey("nodeName") ? sr.Fields["nodeName"] : "",
                            Text = sr.Fields.ContainsKey("description") ? sr.Fields["description"] : "",
                            Date = sr.Fields.ContainsKey("updateDate") ? sr.Fields["updateDate"] : "", 
                        };

                        result.Text = result.Text.Substring(0, Math.Min(result.Text.Length, 250));
                        result.FText = "";
                        if (result.Text.Length > 249 && !result.Text.EndsWith("."))
                        {
                            result.FText = "1";

                        }
                        results.Add(result);
                    }

                }
            }

            //sort it by LastLoginDate this is the default..
            if (results.Count > 0)
                results = results.OrderByDescending(o => o.Date).ToList();


            return results;

        }

        public static string TryMyParse(string text)
        {
            DateTime date;
            if (DateTime.TryParse(text, out date))
            {
                return String.Format("{0:g}", date);
            }
            else
            {
                return "";
            }

        }

        private static string GetUserRoleByValue(string roleNo)
        {
            string _role = string.Empty;
 
            if (String.IsNullOrWhiteSpace(roleNo))
            {
                _role = "No Role Assigned";
            }
            else
            {
                UiEnum.ShipRolesUmbraco valueEnum = (UiEnum.ShipRolesUmbraco)Enum.Parse(typeof(UiEnum.ShipRolesUmbraco), roleNo, true);
                _role = valueEnum.Description();
            }

            return _role;
        }

        private static string GetUserRole(Umbraco.Core.Models.IMember auser)
        {
            string _role = string.Empty;
            string roleNo = auser.GetValue("roleAssigned").ToString();
            if (String.IsNullOrWhiteSpace(roleNo))
            {
                _role = "No Role Assigned";
            }
            else
            {
                UiEnum.ShipRolesUmbraco valueEnum = (UiEnum.ShipRolesUmbraco)Enum.Parse(typeof(UiEnum.ShipRolesUmbraco), roleNo, true);
                _role = valueEnum.Description();
            }

            return _role;
        }



        //NOT USED
        public ActionResult Search(string baz)
        {
            return PartialView("approvesurface/ccAcl");

            UserViewModel model = new UserViewModel();


            List<UserView> users = new List<UserView>();


            users.Add(new UserView
            {
                Name = "test",
                Email = "test@email.com",
                Status = "2009"
            });

            model.UsersView = users;

            return PartialView("approvesurface/ccAcl", model);

        }


        //NOT USED
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HandleListUser(UserViewModel model)
        {

            if (!ModelState.IsValid)
            {
                //return to current page 
                return CurrentUmbracoPage();
            }



            //bool hasloggedin = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;

            //string usern = string.Empty;
            //usern = System.Web.HttpContext.Current.User.Identity.Name;

            var userService = Services.MemberService;

            //var amember = userService.GetByEmail(usern);
            //string state = amember.GetValue("state").ToString();


            string url = "/approve?state=" + model.State;



            return Redirect(url);


        }
    }
}
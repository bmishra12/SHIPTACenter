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


    public class ResourceSurfaceController : SurfaceController
    {

        public ActionResult RenderPendingResource(PendingResourceViewModel model)
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

            model.ResourceView = searchPendingResources(model);

            return PartialView("resourcesurface/resourcerpendinglist", model);
        }


        public ActionResult RenderSearchResource(SearchResourceViewModel model)
        {
            bool hasloggedin = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!hasloggedin)
            {
                return Redirect("/");
            }

            model.UiSubject = new List<Activities>
                    {
                        new Activities { Text = "Appeals" },
                        new Activities { Text = "Claims/Statements" },
                        new Activities { Text = "Coordination of benefits" },
                        new Activities { Text = "Costs" },
                        
                        new Activities { Text = "Enrollment/Eligibility" },
                        new Activities { Text = "Fraud, errors, abuse" },
                        new Activities { Text = "Long‐term care insurance" },
                        new Activities { Text = "Low income" },
                        new Activities { Text = "Marketplace" },
                        new Activities { Text = "Medicaid" },
                        new Activities { Text = "Medicare Advantage" },
                        new Activities { Text = "Medicare cards/numbers" },
                        new Activities { Text = "Part A covered services" },
                        new Activities { Text = "Part B covered services" },
                        new Activities { Text = "Prescription coverage" },        
                        new Activities { Text = "SHIP program" },
                       
                        new Activities { Text = "Social Security" },
                        new Activities { Text = "Supplemental Insurance" }

                    };

            model.UiActivity = new List<Activities>
                    {
                        new Activities { Text = "Counseling" },
                        new Activities { Text = "Outreach" },
                        new Activities { Text = "Program management" },
                        new Activities { Text = "SHIP promotion" },
                        new Activities { Text = "Training" },
                        new Activities { Text = "Volunteer management" }

                    };


            model.UiAudience = new List<Activities>
                    {
                        new Activities { Text = "Beneficiaires" },
                        new Activities { Text = "Caregivers" },
                        new Activities { Text = "Counselors" },
                        new Activities { Text = "Directors" },
                        new Activities { Text = "Disability" },
                        new Activities { Text = "Diverse" },
                        new Activities { Text = "Duals" },
                        new Activities { Text = "Low-income" },
                        new Activities { Text = "Low-literacy" },
                        new Activities { Text = "Low-vision" },
                        new Activities { Text = "Non-english" },
                        new Activities { Text = "Partners" },
                        new Activities { Text = "Rural/frontier" },
                        new Activities { Text = "SHIPs" },
                        new Activities { Text = "Staff" },
                        new Activities { Text = "Volunteers" }
                    };


            model.UiFileTypes = new List<Activities>
                    {
                        new Activities { Text = "Advertisement" },
                        new Activities { Text = "Brochure" },
                        new Activities { Text = "Checklist" },
                        new Activities { Text = "Consumer Guide" },
                        new Activities { Text = "Contract/Agreement" },
                        new Activities { Text = "Fact Sheet" },
                        new Activities { Text = "Form" },
                        
                        new Activities { Text = "Handout" },
                        new Activities { Text = "Image" },

                        new Activities { Text = "Letter" },
                        new Activities { Text = "Logo" },

                        new Activities { Text = "Manual/User Guide" },
                        new Activities { Text = "Other" },
                        
                        new Activities { Text = "Policy/protocol" },
                        new Activities { Text = "PowerPoint" },
                        new Activities { Text = "Report" },
                        new Activities { Text = "Script" },
                        new Activities { Text = "Social Media" },
                        new Activities { Text = "Spreadsheet" },
                        new Activities { Text = "Survey" },

                        new Activities { Text = "Toolkit" },
                        new Activities { Text = "Video" },
                        new Activities { Text = "Webinar" },
                        new Activities { Text = "Website" }
                    };


            model.ResourceView = SearchResources(model);

            return PartialView("resourcesurface/resourcesearch", model);
        }




        private void AddGroupedCriteria(ref IBooleanOperation filter, string field, string value)
        {
            string[] values = value.Split(',');
            var fields = new string[values.Length];
            for (var i = 0; i < values.Length; i++)
            {
                fields[i] = field;
            }
            filter.And().GroupedOr(fields, values);

        }

        private List<SResult> SearchResourcesExact(SearchResourceViewModel model)
        {

            List<SResult> results = new List<SResult>();

            //if nothing selected return null result
            if (model.ResTerm.IsNullOrWhiteSpace()
                && model.SelectedFileTypes == null
                && model.SelectedSubjects == null
                && model.SelectedAudences == null
                && model.SelectedActivities == null
                && model.State.IsNullOrWhiteSpace())
            {
                return results;
            }

            string[] roles = System.Web.Security.Roles.GetRolesForUser(User.Identity.Name);

           // filter = filter.And().Field("GroupAccess", roles[0]);

            var searcher = ExamineManager.Instance.SearchProviderCollection["ResourceSearcher"];


            ISearchCriteria criteria = searcher.CreateSearchCriteria();

            //string fq = string.Format("+description:{0}", model.ResTerm.ToLower());
            // var query = criteria.RawQuery("+description:\"lazy dog\"");


            string fq = string.Format("+GroupAccess:{1} +(description:{0} resoucetitle:{0} nodeName:{0} contributor:{0} filenames:{0})", model.ResTerm.ToLower(), roles[0]);

            var query = criteria.RawQuery(fq);



            ISearchResults SearchResults = searcher.Search(query);


            foreach (var sr in SearchResults)
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

            return results;

        }

        private List<SResult> SearchResources(SearchResourceViewModel model)
        {

            //the resource will be searched only for the logged in users

            List<SResult> results = new List<SResult>();
            bool includeDateRange = false;

            //from date
            DateTime fromDate = System.DateTime.Today;
            //todate
            DateTime toDate = System.DateTime.Today;


            //check if the date is provided if datefrom has some value
            if (!model.DateFrom.IsNullOrWhiteSpace())
            {

                if (!DateTime.TryParse(model.DateFrom, out fromDate))
                {
                    // handle parse failure
                }

                if (!DateTime.TryParse(model.DateTo, out toDate))
                {
                    // handle parse failure
                }
            } 
                
            //if both the from and to is today then ignore the dates.
            if (fromDate == System.DateTime.Today && fromDate == System.DateTime.Today)
                includeDateRange = false; 
            else
                includeDateRange = true;


            // Find pages that contain our search text in either their nodeName or bodyText fields...
            // but exclude any pages that have been hidden.
            // searchCriteria.Fields("nodeName",terms.Boost(8)).Or().Field("metaTitle","hello".Boost(5)).Compile();

            //if nothing selected return null result
            if (model.ResTerm.IsNullOrWhiteSpace()
                && model.SelectedFileTypes == null
                && model.SelectedSubjects == null
                && model.SelectedAudences == null
                && model.SelectedActivities == null
                && model.State.IsNullOrWhiteSpace()
                && includeDateRange == false
                )
            {
                return results;
            }

            if (User.Identity.IsAuthenticated)
            {

                //check if the term has double quotes and not space only
                //this block for exactSearch only...
                if (model.ExactMatch == true && !model.ResTerm.IsNullOrWhiteSpace())
                {
                    //if there is double quotes remove them.
                    model.ResTerm = model.ResTerm.Replace("\"", "");

                    //add the double quotes 
                    model.ResTerm = "\"" + model.ResTerm + "\"";
                    results = SearchResourcesExact(model);
                    return results;
                }



                string[] roles = System.Web.Security.Roles.GetRolesForUser(User.Identity.Name);

                //Resouce search

                // sc.GroupedOr(new string[] { "productCode" },
                //        Examine.LuceneEngine.SearchCriteria.LuceneSearchExtensions.MultipleCharacterWildcard("*" + searchQuery)).Compile();



                //if there is space search for multi word with partial match.
                IExamineValue srcTerm = null;

                if (model.ResTerm.IsNullOrWhiteSpace())
                    srcTerm = null;

                else if (model.ResTerm.Contains(" "))

                    srcTerm = model.ResTerm.ToLower().Escape();
                else
                    srcTerm = model.ResTerm.ToLower().MultipleCharacterWildcard();



                var criteriaResourceRestricted = ExamineManager.Instance
                 .SearchProviderCollection["ResourceSearcher"]
                  .CreateSearchCriteria();
                 
                 //.CreateSearchCriteria();

                //          .CreateSearchCriteria(BooleanOperation.Or);

                Examine.SearchCriteria.IBooleanOperation filter = null;


                filter = criteriaResourceRestricted.Field("IsPublic", "false"); //just to start a dummy this is OR

                filter = filter.And().Field("GroupAccess", roles[0]);

                //Added contributor and  filenames
                if (!model.ResTerm.IsNullOrWhiteSpace() && model.ResTerm.Length > 0) //if ther is nothing in resterm ignore the line
                    filter = filter.And().GroupedOr(new string[] { "description", "resoucetitle", "nodeName", "name", "seo", "resourcetypes", "subjects", "intendedaudiences", "activitys" , "contributor" , "filenames"}, srcTerm);

                if (model.SelectedFileTypes != null)
                    AddGroupedCriteria(ref filter, "resourcetypes", string.Join(",", model.SelectedFileTypes));

                if (model.SelectedSubjects != null)
                    AddGroupedCriteria(ref filter, "subjects", string.Join(",", model.SelectedSubjects));

                if (model.SelectedAudences != null)
                    AddGroupedCriteria(ref filter, "intendedaudiences", string.Join(",", model.SelectedAudences));

                if (model.SelectedActivities != null)
                    AddGroupedCriteria(ref filter, "activitys", string.Join(",", model.SelectedActivities));

                if (model.State != null && model.State.Length > 0)
                    filter = filter.And().Field("state", model.State);




                if (includeDateRange == true)
                {
                    filter = filter.And()
                         .Range("updateDate",
                           fromDate.ToString("yyyy-MM-ddTHH:mm:ss", System.Globalization.CultureInfo.InvariantCulture),
                           toDate.ToString("yyyy-MM-ddTHH:mm:ss", System.Globalization.CultureInfo.InvariantCulture),
                            true, true);

                }


                //sort by updated date NOT WORKING..
                //filter = filter.And().OrderBy("updateDate");


                ISearchResults SearchResults = ExamineManager.Instance
                    .SearchProviderCollection["ResourceSearcher"]
                    .Search(filter.Compile());


                foreach (var sr in SearchResults)
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


            //sort it by LastLoginDate this is the default..
            if (results.Count > 0)
                results = results.OrderByDescending(o => o.Date).ToList();



            return results;




        }

        private List<ResourceView> searchPendingResources(PendingResourceViewModel model)
        {
            List<ResourceView> resources = new List<ResourceView>();

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


            foreach (var auser in items)
            {
                //do not include the loggedin user..

                resources.Add(
                            new ResourceView
                            {
                                ID = auser.Id.ToString(),
                                //Name = auser.Name,
                                Name = auser.Name,
                                Contributor = auser.GetValue("contributor").ToString(),
                                CreatedDate = TryMyParse(auser.CreateDate.ToString()),
                                Status = "",
                                State = auser.GetValue("state").ToString()
                            }
                            );
            }


            return resources;
        }


        private List<ResourceView> searchPendingResourcesOLDNotused(PendingResourceViewModel model)
        {
            List<ResourceView> resources = new List<ResourceView>();

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


            //@if( @Roles.IsUserInRole("StateAdmin"))
            var pageSize = 8;
            var page = 1; int.TryParse(Request.QueryString["page"], out page);
            //var items = Model.Content.Children().Where(x => x.IsDocumentType("textpage")).OrderByDescending(x => x.CreateDate);
            //var date = DateTime.UtcNow.AddMinutes(-AppConstants.TimeSpanInMinutesToShowMembers);
            // var members = ApplicationContext.Current.Services.MemberService.GetMembersByPropertyValue(AppConstants.PropMemberLastActiveDate, date, ValuePropertyMatchType.GreaterThan)
            //                 .Where(x => x.IsApproved && !x.IsLockedOut);



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

                resources.Add(
                            new ResourceView
                            {
                                ID = auser.Id.ToString(),
                                //Name = auser.Name,
                                Name = auser.Name,
                                Contributor = auser.GetValue("contributor").ToString(),
                                CreatedDate = TryMyParse(auser.CreateDate.ToString()),
                                Status = "",
                                State = auser.GetValue("state").ToString()
                            }
                            );
            }


            return resources;
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

        public ActionResult RenderAddResource()
        {
            bool hasloggedin = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!hasloggedin)
            {
                return PartialView("authsurface/notauth");
            }

            if (System.Web.HttpContext.Current.User.IsInRole("shiptraining")
                || System.Web.HttpContext.Current.User.IsInRole("shipcounselor")
                || System.Web.HttpContext.Current.User.IsInRole("partner")

                )
                return PartialView("authsurface/notauth");
            //redirect can not happen in Render..
            //return Redirect("/notauthorized");


            ResourceViewModel model = new ResourceViewModel();


            model.UiSubject = new List<Activities>
                    {
                        new Activities { Text = "Appeals" },
                        new Activities { Text = "Claims/Statements" },
                        new Activities { Text = "Coordination of benefits" },
                        new Activities { Text = "Costs" },
                        
                        new Activities { Text = "Enrollment/Eligibility" },
                        new Activities { Text = "Fraud, errors, abuse" },
                        new Activities { Text = "Long‐term care insurance" },
                        new Activities { Text = "Low income" },
                        new Activities { Text = "Marketplace" },
                        new Activities { Text = "Medicaid" },
                        new Activities { Text = "Medicare Advantage" },
                        new Activities { Text = "Medicare cards/numbers" },
                        new Activities { Text = "Part A covered services" },
                        new Activities { Text = "Part B covered services" },
                        new Activities { Text = "Prescription coverage" },        
                        new Activities { Text = "SHIP program" },
                       
                        new Activities { Text = "Social Security" },
                        new Activities { Text = "Supplemental Insurance" }

                    };

            model.UiActivity = new List<Activities>
                    {
                        new Activities { Text = "Counseling" },
                        new Activities { Text = "Outreach" },
                        new Activities { Text = "Program management" },
                        new Activities { Text = "SHIP promotion" },
                        new Activities { Text = "Training" },
                        new Activities { Text = "Volunteer management" }

                    };


            model.UiAudience = new List<Activities>
                    {
                        new Activities { Text = "Beneficiaires" },
                        new Activities { Text = "Caregivers" },
                        new Activities { Text = "Counselors" },
                        new Activities { Text = "Directors" },
                        new Activities { Text = "Disability" },
                        new Activities { Text = "Diverse" },
                        new Activities { Text = "Duals" },
                        new Activities { Text = "Low-income" },
                        new Activities { Text = "Low-literacy" },
                        new Activities { Text = "Low-vision" },
                        new Activities { Text = "Non-english" },
                        new Activities { Text = "Partners" },
                        new Activities { Text = "Rural/frontier" },
                        new Activities { Text = "SHIPs" },
                        new Activities { Text = "Staff" },
                        new Activities { Text = "Volunteers" }
                    };


            model.UiFileTypes = new List<Activities>
                    {
                        new Activities { Text = "Advertisement" },
                        new Activities { Text = "Brochure" },
                        new Activities { Text = "Checklist" },
                        new Activities { Text = "Consumer Guide" },
                        new Activities { Text = "Contract/Agreement" },
                        new Activities { Text = "Fact Sheet" },
                        new Activities { Text = "Form" },
                        
                        new Activities { Text = "Handout" },
                        new Activities { Text = "Image" },

                        new Activities { Text = "Letter" },
                        new Activities { Text = "Logo" },

                        new Activities { Text = "Manual/User Guide" },
                        new Activities { Text = "Other" },
                        
                        new Activities { Text = "Policy/protocol" },
                        new Activities { Text = "PowerPoint" },
                        new Activities { Text = "Report" },
                        new Activities { Text = "Script" },
                        new Activities { Text = "Social Media" },
                        new Activities { Text = "Spreadsheet" },
                        new Activities { Text = "Survey" },

                        new Activities { Text = "Toolkit" },
                        new Activities { Text = "Video" },
                        new Activities { Text = "Webinar" },
                        new Activities { Text = "Website" }
                    };

            return PartialView("resourcesurface/resourcecreate", model);
        }



        //this resourceid is sent by the querystring
        public ActionResult RenderApproveResource(int resourceid)
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

            ResourceApproveViewModel model = new ResourceApproveViewModel();

            //get the resource and populate all the model properties..
            var cs = ApplicationContext.Current.Services.ContentService;

            var con = cs.GetById(resourceid);

            //Get all the values
            model.Title = con.GetValue("resoucetitle").ToString();

            model.Description = con.GetValue("description").ToString();
            string strSubjects = con.GetValue("subjects").ToString();
            string strActivitys = con.GetValue("activitys").ToString();
            string strAudiences = con.GetValue("intendedaudiences").ToString();
            string strTypes = con.GetValue("resourcetypes").ToString();

           // string MediaIds = con.GetValue("mediaids").ToString(); //this line throws error if the mediaid is null
           
            //check for null then get the string.
            string MediaIds = (con.GetValue("mediaids") ?? String.Empty).ToString();
            
            model.MediaUrls = new List<UrlLink>();


            if (MediaIds.IsNullOrWhiteSpace())
            {
                model.MediaUrls = null;
            }
            else
            {
                //mediaId is the ID returned from your document property...
                var ms = ApplicationContext.Current.Services.MediaService;

                foreach (string amediaId in MediaIds.Split(','))
                {
                    if (!amediaId.IsNullOrWhiteSpace())
                    {
                        Umbraco.Core.Models.IMedia f = ms.GetById(Convert.ToInt32(amediaId));
                        if (f != null)
                        {
                            model.MediaUrls.Add(new UrlLink { FileName = f.Name, FileUrl = f.Properties["umbracoFile"].Value.ToString() });
                        }
                    }
                }
            }

            model.State = con.GetValue("state").ToString();

            model.Contributor = con.GetValue("contributor").ToString();
            model.ContributorEmail = con.GetValue("contributoremail").ToString();

            //String s = (myObj ?? String.Empty).ToString();
            //check for null then apply the string.
            string links = (con.GetValue("links") ?? String.Empty).ToString();
            if (links.IsNullOrWhiteSpace())
            {
                model.MyRlinks = null;
            }
            else
            {
                model.MyRlinks = new JavaScriptSerializer().Deserialize<List<RLinks>>(links);
            }


            model.UiSubject = new List<Activities>
                    {
                        new Activities { Text = "Appeals", Checked =strSubjects.Contains("Appeals")  },
                        new Activities { Text = "Claims/Statements" , Checked =strSubjects.Contains("Claims") },
                        new Activities { Text = "Coordination of benefits" , Checked =strSubjects.Contains("Coordination") },
                        new Activities { Text = "Costs" , Checked =strSubjects.Contains("Costs") },


                        new Activities { Text = "Enrollment/Eligibility" , Checked =strSubjects.Contains("Enrollment") },
                        new Activities { Text = "Fraud, errors, abuse", Checked =strSubjects.Contains("Fraud")  },
                        new Activities { Text = "Long‐term care insurance" , Checked =strSubjects.Contains("insurance") },
                        new Activities { Text = "Low income", Checked =strSubjects.Contains("income")  },
                        new Activities { Text = "Marketplace" , Checked =strSubjects.Contains("Marketplace") },
                        new Activities { Text = "Medicaid", Checked =strSubjects.Contains("Medicaid")  },
                        new Activities { Text = "Medicare Advantage", Checked =strSubjects.Contains("Advantage")  },
                        new Activities { Text = "Medicare cards/numbers" , Checked =strSubjects.Contains("numbers") },
                        new Activities { Text = "Part A covered services" , Checked =strSubjects.Contains("Part A") },
                        new Activities { Text = "Part B covered services", Checked =strSubjects.Contains("Part B")  },
                        new Activities { Text = "Prescription coverage", Checked =strSubjects.Contains("Prescription")  },
                        new Activities { Text = "SHIP program", Checked =strSubjects.Contains("program")  },

                        new Activities { Text = "Social Security", Checked =strSubjects.Contains("Social")  },
                        new Activities { Text = "Supplemental Insurance", Checked =strSubjects.Contains("Supplemental")  }

                    };


            model.UiActivity = new List<Activities>
                    {
                        new Activities { Text = "Counseling", Checked =strActivitys.Contains("Counseling") },
                        new Activities { Text = "Outreach", Checked =strActivitys.Contains("Outreach") },
                        new Activities { Text = "Program management" , Checked =strActivitys.Contains("Program")},
                        new Activities { Text = "SHIP promotion", Checked =strActivitys.Contains("SHIP") },
                        new Activities { Text = "Training" , Checked =strActivitys.Contains("Training")},
                        new Activities { Text = "Volunteer management", Checked =strActivitys.Contains("Volunteer") }

                    };
            model.UiAudience = new List<Activities>
                    {
                        new Activities { Text = "Beneficiaires", Checked =strAudiences.Contains("Beneficiaires")  },
                        new Activities { Text = "Caregivers", Checked =strAudiences.Contains("Caregivers")  },
                        new Activities { Text = "Counselors" , Checked =strAudiences.Contains("Counselors") },
                        new Activities { Text = "Directors", Checked =strAudiences.Contains("Directors")  },
                        new Activities { Text = "Disability", Checked =strAudiences.Contains("Disability")  },
                        new Activities { Text = "Diverse", Checked =strAudiences.Contains("Diverse")  },
                        new Activities { Text = "Duals", Checked =strAudiences.Contains("Duals")  },
                        new Activities { Text = "Low-income" , Checked =strAudiences.Contains("income") },
                        new Activities { Text = "Low-literacy", Checked =strAudiences.Contains("literacy")  },
                        new Activities { Text = "Low-vision" , Checked =strAudiences.Contains("vision") },
                        new Activities { Text = "Non-english", Checked =strAudiences.Contains("english")  },
                        new Activities { Text = "Partners" , Checked =strAudiences.Contains("Partners") },
                        new Activities { Text = "Rural/frontier" , Checked =strAudiences.Contains("Rural") },
                        new Activities { Text = "SHIPs" , Checked =strAudiences.Contains("SHIPs") },
                        new Activities { Text = "Staff" , Checked =strAudiences.Contains("Staff") },
                        new Activities { Text = "Volunteers" , Checked =strAudiences.Contains("Volunteers") }
                    };
            model.UiFileTypes = new List<Activities>
                    {
                        new Activities { Text = "Advertisement" , Checked =strTypes.Contains("Advertisement") },
                        new Activities { Text = "Brochure", Checked =strTypes.Contains("Brochure")  },
                        new Activities { Text = "Checklist" , Checked =strTypes.Contains("Checklist") },

                        new Activities { Text = "Consumer Guide" , Checked =strTypes.Contains("Consumer") },
                        new Activities { Text = "Contract/Agreement", Checked =strTypes.Contains("Contract")  },

                        new Activities { Text = "Fact Sheet" , Checked =strTypes.Contains("Fact") },
                        new Activities { Text = "Form" , Checked =strTypes.Contains("Form") },
                        new Activities { Text = "Handout" , Checked =strTypes.Contains("Handout") },
                        new Activities { Text = "Image" , Checked =strTypes.Contains("Image") },

                        new Activities { Text = "Letter" , Checked =strTypes.Contains("Letter") },
                        new Activities { Text = "Logo" , Checked =strTypes.Contains("Logo") },

                        new Activities { Text = "Manual/User Guide", Checked =strTypes.Contains("Manual")  },
                        new Activities { Text = "Other" , Checked =strTypes.Contains("Other") },

                        new Activities { Text = "Policy/protocol", Checked =strTypes.Contains("Policy")  },
                        new Activities { Text = "PowerPoint", Checked =strTypes.Contains("PowerPoint")  },
                        new Activities { Text = "Report" , Checked =strTypes.Contains("Report") },
                        new Activities { Text = "Script", Checked =strTypes.Contains("Script")  },
                        new Activities { Text = "Social Media", Checked =strTypes.Contains("Social")  },

                        new Activities { Text = "Spreadsheet", Checked =strTypes.Contains("Spreadsheet")  },
                        new Activities { Text = "Survey", Checked =strTypes.Contains("Survey")  },

                        new Activities { Text = "Toolkit" , Checked =strTypes.Contains("Toolkit") },
                        new Activities { Text = "Video" , Checked =strTypes.Contains("Video") },
                        new Activities { Text = "Webinar", Checked =strTypes.Contains("Webinar")  },
                        new Activities { Text = "Website", Checked =strTypes.Contains("Website")  }
                    };


            return PartialView("resourcesurface/resourceapprove", model);

        }




        [HttpPost]
        public ActionResult HandleAddResource(ResourceViewModel model)
        {
            bool isShipAdmin = false;
            bool isStateAdmin = false;
            string fname = string.Empty;
            string lname = string.Empty;
            string userName = string.Empty;
            string userEmail = System.Web.HttpContext.Current.User.Identity.Name;

            if (!ModelState.IsValid)
            {
                //return to current page 
                return CurrentUmbracoPage();
            }

            // create the RLinks list
            var relatedLinks = new List<RLinks>();

            if (System.Web.HttpContext.Current.User.IsInRole("shipcenter")) isShipAdmin = true;

            if (System.Web.HttpContext.Current.User.IsInRole("shipdirector") || System.Web.HttpContext.Current.User.IsInRole("acladmin") || System.Web.HttpContext.Current.User.IsInRole("shipadmin")) isStateAdmin = true;


            if ((model.myCaption1 != null) && (model.myLink1 != null))
            {
                RLinks alink1 = new RLinks()
                {
                    caption = model.myCaption1,
                    edit = false,
                    link = model.myLink1,
                    isInternal = false,
                    newWindow = true,
                    title = "title ",
                    type = "external"
                };
                relatedLinks.Add(alink1);
            }
            if ((model.myCaption2 != null) && (model.myLink2 != null))
            {
                RLinks alink2 = new RLinks()
                {
                    caption = model.myCaption2,
                    edit = false,
                    link = model.myLink2,
                    isInternal = false,
                    newWindow = true,
                    title = "title ",
                    type = "external"
                };
                relatedLinks.Add(alink2);
            }


            //check all the checkbox counts.
            var selectedSubjects = model.UiSubject.Where(rsp => rsp.Checked);

            var selectedActivitys = model.UiActivity.Where(rsp => rsp.Checked);

            var selectedAudiences = model.UiAudience.Where(rsp => rsp.Checked);

            var selectedTypes = model.UiFileTypes.Where(rsp => rsp.Checked);



            if (selectedSubjects.Count() < 1)
            {
                ModelState.AddModelError("ResourceViewModel", "You have to select at least one subject. ");
            }
            if (selectedActivitys.Count() < 1)
            {
                ModelState.AddModelError("ResourceViewModel", "You have to select at least one activity. ");
            }
            if (selectedAudiences.Count() < 1)
            {
                ModelState.AddModelError("ResourceViewModel", "You have to select at least one audience. ");
            }
            if (selectedTypes.Count() < 1)
            {
                ModelState.AddModelError("ResourceViewModel", "You have to select at least one type.");
            }

            if (isShipAdmin)
            {
                if (model.State.IsNullOrWhiteSpace())
                {
                    ModelState.AddModelError("ResourceViewModel", "You have to select a State.");
                }
            }
            if (isShipAdmin || isStateAdmin)
            {
                if (model.Role.IsNullOrWhiteSpace())
                {
                    ModelState.AddModelError("ResourceViewModel", "You have to select a Role.");
                }
            }



            //check if file uploaded?
            bool isFileUploaded = false;
            foreach (var afile in model.File)
            {
                if (afile != null && afile.ContentLength > 0)
                //we can chek the media type here and filter..
                {
                    isFileUploaded = true;
                    break;
                }
            }

            if (!isFileUploaded && relatedLinks.Count() == 0)
            {
                ModelState.AddModelError("ResourceViewModel", "A minimum of one file or one link is required.");
            }

            if (ModelState["ResourceViewModel"] != null)
            {
                var errCount = ModelState["ResourceViewModel"].Errors.Count;
                if (errCount > 0)
                {
                    //Return the view...
                    return CurrentUmbracoPage();

                }
            }



            var json = new JavaScriptSerializer().Serialize(relatedLinks);


            //get the newly created id..
            List<int> mediaIds = new List<int>();

            //store the filename..
            List<string> filenames = new List<string>();

            int mediaRootId = ConfigUtil.MediaRootId;

            //add all the uploaded files to media folder and collects the MediaIDS..
            foreach (var afile in model.File)
            {


                if (afile != null && afile.ContentLength > 0)
                //we can chek the media type here and filter..
                {
                    try
                    {
                        var fileName = afile.FileName;


                        var mediaType = Constants.Conventions.MediaTypes.File;

                        var mediaService = ApplicationContext.Current.Services.MediaService;


                        var ext = fileName.Substring(fileName.LastIndexOf('.') + 1).ToLower();

                        if ("jpeg,jpg,gif,bmp,png,tiff,tif".Contains(ext))
                        {
                            mediaType = global::Umbraco.Core.Constants.Conventions.MediaTypes.Image;
                        }

                        var myFileName = System.IO.Path.GetFileName(fileName);



                        Umbraco.Core.Models.IMedia f = mediaService.CreateMedia(myFileName, mediaRootId, mediaType);

                        // Assumes the image._Image is a Stream - you may have to do some extra work here...
                        f.SetValue(Constants.Conventions.Media.File, afile); // Real magic happens here.

                        mediaService.Save(f);

                        //check the f.id to get the valu  
                        if (f.Id > 0)
                        {
                            mediaIds.Add(f.Id);
                            filenames.Add(fileName);
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
                }
            }
            //add the resource and populate the rlinks and mediaFile IDS

            string strSubjects = String.Join(",", selectedSubjects.Select(x => x.Text));
            string strActivitys = String.Join(",", selectedActivitys.Select(x => x.Text));
            string strAudiences = String.Join(",", selectedAudiences.Select(x => x.Text));
            string strTypes = String.Join(",", selectedTypes.Select(x => x.Text));
            string strMedias = String.Join(",", mediaIds.ToArray());
            string strFilenames = String.Join(",", filenames.ToArray());

            //create the content..

            //chose the correct ResourceNode
            //if the user role is shipCenter then pick up the intended role chosen.
            //else it is unpublished node..
            int parentID = 0;
            string state = "";

            if (isShipAdmin || isStateAdmin)
            {


                if (model.Role == "1")
                {
                    parentID = ConfigUtil.ResourceR1Id;
                }
                else if (model.Role == "2")
                {
                    parentID = ConfigUtil.ResourceR2Id;
                }
                else if (model.Role == "3")
                {
                    parentID = ConfigUtil.ResourceR3Id;
                }
                else if (model.Role == "4")
                {
                    parentID = ConfigUtil.ResourceR4Id;
                }
                else //This situation will never occur but just in case default - visible to larger audience
                {
                    parentID = ConfigUtil.ResourceR1Id;
                }


            }
            else
            {
                //state is logged in user state..
                parentID = ConfigUtil.ResourceUnpublishedId;

            }


            var userService = ApplicationContext.Current.Services.MemberService;
            var amember = userService.GetByEmail(userEmail);
            string astate = amember.GetValue("state").ToString();
            fname = amember.GetValue("firstName").ToString();
            lname = amember.GetValue("lastName").ToString();
            userName = string.Format("{0} {1}", fname, lname);


            if (isShipAdmin)
            {
                state = model.State;
            }
            else
            {
                state = astate;
            }

            int userID = 0;

            Umbraco.Core.Services.ContentService csv = new Umbraco.Core.Services.ContentService();


            var con = csv.CreateContent(model.Title, parentID, "Resource", userID);

            //set all the values
            con.SetValue("resoucetitle", model.Title);

            con.SetValue("description", model.Description);

            con.SetValue("subjects", strSubjects);
            con.SetValue("activitys", strActivitys);
            con.SetValue("intendedaudiences", strAudiences);
            con.SetValue("resourcetypes", strTypes);

            con.SetValue("mediaids", strMedias);

            con.SetValue("filenames", strFilenames);

            con.SetValue("state", state);

            con.SetValue("contributor", userName);
            con.SetValue("contributoremail", userEmail);



            if (relatedLinks.Count > 0)
            {
                con.SetValue("links", json);
            }

            con.SetValue("mediaids", strMedias);

            if (isShipAdmin || isStateAdmin)
            {
                csv.SaveAndPublishWithStatus(con);
            }
            else
            {
                //save the document and send the approver email to publish..
                csv.Save(con);

                //send email to approver

                //Send out verification email, with GUID in it
                EmailHelper email = new EmailHelper();
                if (email.SendResourceWaitingForApproval(userEmail, fname) == false)
                {
                    //show  error in  page email couldnot send

                    //Return the view...
                    TempData["success"] = false;
                    //  return CurrentUmbracoPage();
                }


                if (email.SendDesignatedNotificationForResourceApproval(state, fname, lname) == false)
                {
                    //show  error in  page email couldnot send
                    //Return the view...
                    TempData["success"] = false;
                    return CurrentUmbracoPage();

                }
            }

            //Return the view...

            TempData["rID"] = con.Id;
            TempData["success"] = true;
            return CurrentUmbracoPage();



        }

        [HttpPost]
        public ActionResult HandleApproveResource(ResourceApproveViewModel model)
        {
            //check what button pressed
            if (model.SubmitAction == "Approve")
            {

                if (!ModelState.IsValid)
                {
                    //return to current page 
                    return CurrentUmbracoPage();
                }

                //if (System.Web.HttpContext.Current.User.IsInRole("shipcenter")) isAdmin = true;


                //check all the checkbox counts.
                var selectedSubjects = model.UiSubject.Where(rsp => rsp.Checked);

                var selectedActivitys = model.UiActivity.Where(rsp => rsp.Checked);

                var selectedAudiences = model.UiAudience.Where(rsp => rsp.Checked);

                var selectedTypes = model.UiFileTypes.Where(rsp => rsp.Checked);



                if (selectedSubjects.Count() < 1)
                {
                    ModelState.AddModelError("ResourceViewModel", "You have to select at least one subject. ");
                }
                if (selectedActivitys.Count() < 1)
                {
                    ModelState.AddModelError("ResourceViewModel", "You have to select at least one activity. ");
                }
                if (selectedAudiences.Count() < 1)
                {
                    ModelState.AddModelError("ResourceViewModel", "You have to select at least one audience. ");
                }
                if (selectedTypes.Count() < 1)
                {
                    ModelState.AddModelError("ResourceViewModel", "You have to select at least one type.");
                }


                if (model.Role.IsNullOrWhiteSpace())
                {
                    ModelState.AddModelError("ResourceViewModel", "You have to select a Role.");
                }


                if (ModelState["ResourceViewModel"] != null)
                {
                    var errCount = ModelState["ResourceViewModel"].Errors.Count;
                    if (errCount > 0)
                    {
                        //Return the view...
                        return CurrentUmbracoPage();

                    }
                }


                Umbraco.Core.Services.ContentService csv = new Umbraco.Core.Services.ContentService();
                var con = csv.GetById(model.ResourceId);

                //check if the content is Unpublished
                if (con.Status.ToString() == "Unpublished")
                {

                    //add the resource and populate the rlinks and mediaFile IDS

                    string strSubjects = String.Join(",", selectedSubjects.Select(x => x.Text));
                    string strActivitys = String.Join(",", selectedActivitys.Select(x => x.Text));
                    string strAudiences = String.Join(",", selectedAudiences.Select(x => x.Text));
                    string strTypes = String.Join(",", selectedTypes.Select(x => x.Text));

                    //create the content..

                    //chose the correct ResourceNode
                    //if the user role is shipCenter then pick up the intended role chosen.
                    int parentID = 0;

                    if (model.Role == "1")
                    {
                        parentID = ConfigUtil.ResourceR1Id;
                    }
                    else if (model.Role == "2")
                    {
                        parentID = ConfigUtil.ResourceR2Id;
                    }
                    else if (model.Role == "3")
                    {
                        parentID = ConfigUtil.ResourceR3Id;
                    }
                    else if (model.Role == "4")
                    {
                        parentID = ConfigUtil.ResourceR4Id;
                    }
                    else //This situation will never occur but just in case default - visible to larger audience
                    {
                        parentID = ConfigUtil.ResourceR1Id;
                    }


                    int userID = 0;


                    csv.Move(con, parentID);

                    //set all the values

                    //set all the values
                    con.SetValue("resoucetitle", model.Title);

                    con.SetValue("description", model.Description);

                    con.SetValue("subjects", strSubjects);
                    con.SetValue("activitys", strActivitys);
                    con.SetValue("intendedaudiences", strAudiences);
                    con.SetValue("resourcetypes", strTypes);


                    con.SetValue("approveremail", System.Web.HttpContext.Current.User.Identity.Name);
                    con.SetValue("approveddate", DateTime.Now.ToString());

                    csv.SaveAndPublishWithStatus(con);


                    //send approved email to contributer 
                    EmailHelper email = new EmailHelper();
                    if (email.SendResourceApproval(model.ContributorEmail, model.Contributor) == false)
                    {
                        //show  error in  page email couldnot send

                        //Return the view...
                        TempData["success"] = false;
                        TempData["message"] = "The email could not be sent.";

                        return CurrentUmbracoPage();
                    }



                    //Return the view...

                    TempData["rID"] = con.Id;
                    TempData["success"] = true;
                    return CurrentUmbracoPage();

                }
                else //the content status is not unpublished
                {
                    TempData["success"] = false;
                    TempData["message"] = "The content is already published.";

                    return CurrentUmbracoPage();


                }



            }
            else if (model.SubmitAction == "Deny")
            {
                Umbraco.Core.Services.ContentService csv = new Umbraco.Core.Services.ContentService();
                var con = csv.GetById(model.ResourceId);

                //check if the content is Unpublished
                if (con.Status.ToString() == "Unpublished")
                {

                    //check for null then get the string.
                    string mediaIds = (con.GetValue("mediaids") ?? String.Empty).ToString();

                    var ms = ApplicationContext.Current.Services.MediaService;

                    //delete all the media..

                    foreach (string amediaId in mediaIds.Split(','))
                    {
                        if (!amediaId.IsNullOrWhiteSpace())
                        {
                            Umbraco.Core.Models.IMedia f = ms.GetById(Convert.ToInt32(amediaId));
                            if (f != null)
                            {
                                ms.Delete(f);
                            }
                        }
                    }

                    //delete the content..
                    csv.Delete(con);

                    //send denial email to contributer 
                    EmailHelper email = new EmailHelper();
                    if (email.SendResourceDenial(model.ContributorEmail, model.Contributor) == false)
                    {
                        //show  error in  page email couldnot send

                        //Return the view...
                        TempData["success"] = false;
                        TempData["message"] = "The email could not be sent.";

                        return CurrentUmbracoPage();
                    }

                    //Return the view...
                    TempData["success"] = true;
                    TempData["message"] = "The resource has been deleted.";

                    return CurrentUmbracoPage();
                }

                TempData["success"] = false;
                TempData["message"] = "The content is already published...";
                return CurrentUmbracoPage();

            }

            else //fall back..
            {
                TempData["success"] = false;
                return CurrentUmbracoPage();
            }


        }



        public ActionResult RenderResourceTopFeatured(ResourceTopViewModel model)
        {

            model.TopListView = GetTopFeaturedResources(model);

            return PartialView("resourcesurface/resourcefeatured", model);
        }

        private List<SResult> GetTopFeaturedResources(ResourceTopViewModel model)
        {
            List<SResult> resources = new List<SResult>();


            Dictionary<string, int> featureResourceIds = new Dictionary<string, int>();


            int resourceFeatureItem = ConfigUtil.ResourceFeatureId;

            Umbraco.Core.Models.IContent item = Services.ContentService.GetById(resourceFeatureItem);

            if (!item.GetValue("feature1").ToString().IsNullOrWhiteSpace())
                featureResourceIds.Add(item.GetValue("feature1").ToString(), 1);

            if (!item.GetValue("feature2").ToString().IsNullOrWhiteSpace())
                featureResourceIds.Add(item.GetValue("feature2").ToString(), 2);

            if (!item.GetValue("feature3").ToString().IsNullOrWhiteSpace())
                featureResourceIds.Add(item.GetValue("feature3").ToString(), 3);

            if (!item.GetValue("feature4").ToString().IsNullOrWhiteSpace())
                featureResourceIds.Add(item.GetValue("feature4").ToString(), 4);

            if (!item.GetValue("feature5").ToString().IsNullOrWhiteSpace())
                featureResourceIds.Add(item.GetValue("feature5").ToString(), 5);

            if (!item.GetValue("feature6").ToString().IsNullOrWhiteSpace())
                featureResourceIds.Add(item.GetValue("feature6").ToString(), 6);

            if (!item.GetValue("feature7").ToString().IsNullOrWhiteSpace())
                featureResourceIds.Add(item.GetValue("feature7").ToString(), 7);

            if (!item.GetValue("feature8").ToString().IsNullOrWhiteSpace())
                featureResourceIds.Add(item.GetValue("feature8").ToString(), 8);

            if (!item.GetValue("feature9").ToString().IsNullOrWhiteSpace())
                featureResourceIds.Add(item.GetValue("feature9").ToString(), 9);

            if (!item.GetValue("feature10").ToString().IsNullOrWhiteSpace())
                featureResourceIds.Add(item.GetValue("feature10").ToString(), 10);

            // IEnumerable<Umbraco.Core.Models.IContent> items = Services.ContentService.GetByIds(featureResourceIds);


            foreach (var fRid in featureResourceIds)
            {

                //get the Item

                var aItem = Services.ContentService.GetById(Convert.ToInt32(fRid.Key));
                if (umbraco.library.HasAccess(aItem.Id, aItem.Path))
                {
                    SResult result = new SResult()
                        {
                            Order = fRid.Value,
                            Id = aItem.Id.ToString(),
                            Url = umbraco.library.NiceUrl(Convert.ToInt32(aItem.Id.ToString())),
                            NodeName = aItem.Name,
                            Text = aItem.GetValue("description").ToString()
                        };

                    result.FText = "";

                    result.Text = result.Text.Substring(0, Math.Min(result.Text.Length, 250));
                    if (result.Text.Length > 249 && !result.Text.EndsWith("."))
                    {
                        result.FText = "1";

                    }

                    resources.Add(result);
                }

            }



            return resources;
        }

        public ActionResult RenderResourceTopRecent(ResourceTopViewModel model)
        {

            model.TopListView = GetTopRecentResources(model);

            return PartialView("resourcesurface/resourcerecent", model);
        }

        private List<SResult> GetTopRecentResources(ResourceTopViewModel model)
        {
            int resourceRootId = 0;

            List<SResult> resources = new List<SResult>();

            resourceRootId = UmbracoShipTac.Code.ConfigUtil.ResourceRootId;



            //TODO decide which one is root resource id from the roleasigned


            IEnumerable<Umbraco.Core.Models.IContent> items;
            items = Services.ContentService.GetDescendants(resourceRootId).Where(x => x.ContentType.Alias == "Resource" && umbraco.library.HasAccess(x.Id, x.Path)).OrderByDescending(x => x.CreateDate).Take(10);
            foreach (var aItem in items)
            {

                SResult result = new SResult()
                {
                    Id = aItem.Id.ToString(),
                    Url = umbraco.library.NiceUrl(Convert.ToInt32(aItem.Id.ToString())),
                    NodeName = aItem.Name,
                    Text = aItem.GetValue("description").ToString()
                };

                result.Text = result.Text.Substring(0, Math.Min(result.Text.Length, 250));

                result.FText = "";
                if (result.Text.Length > 249 && !result.Text.EndsWith("."))
                {
                    result.FText = "1";

                }



                resources.Add(result);
            }




            return resources;
        }
    }
}

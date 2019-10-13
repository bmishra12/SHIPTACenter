using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Umbraco.Web.Mvc;

using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.member;
using UmbracoShipTac.Code;
using System.Text;
using UmbracoShipTac.Models;
using Umbraco.Core.Persistence.Querying;


namespace UmbracoShipTac.Controllers
{
    public class ShipTacController : SurfaceController
    {
        //public ActionResult DoSomething(String myParam, String returnUrl)
        //{
        //    /* Do something.... */

        //    String url = HttpUtility.UrlDecode(returnUrl.Replace("--", "%"));
        //    return Redirect(url);
        //}






        public ActionResult RenderApproveMember(string EmailId)
        {

            bool hasloggedin = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
         
            if (!hasloggedin)  //check also if he has stateadmin or acladmin role
            {
                return Redirect("/");
            }

            var Imember = Services.MemberService.GetByEmail(EmailId);

            //Ensure we find a member with the verifyGUID
            if (Imember != null)
            {

               //// Services.MemberService.AssignRole(Imember.Email, "RegisteredMembers");

                Imember.IsApproved = true;
                Services.MemberService.Save(Imember);


                //Send user an email to reset password with GUID in it
                EmailHelper email = new EmailHelper();


                if (email.SendApprovedEmail(EmailId, Imember.Name))
                {

                    return Redirect("/approve");
                }
                else //approved email could not send..
                {
                    return Redirect("/");
                }


            }
            else
            {
                //Couldn't find them - most likely invalid GUID
                return Redirect("/");
            }

        }

        public ActionResult RenderDenyMember(string EmailId)
        {

            bool hasloggedin = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;

            if (!hasloggedin)  //check also if he has stateadmin or acladmin role
            {
                return Redirect("/");
            }

            var Imember = Services.MemberService.GetByEmail(EmailId);

            //Ensure we find a member with the verifyGUID
            if (Imember != null)
            {

                //// Services.MemberService.AssignRole(Imember.Email, "RegisteredMembers");

                Services.MemberService.Delete(Imember);


                //Send user an email to reset password with GUID in it
                EmailHelper email = new EmailHelper();


                if (email.SendDisapproveEmail(EmailId, Imember.Name))
                {

                    return Redirect("/approve");
                }
                else //approved email could not send..
                {
                    return Redirect("/");
                }


            }
            else
            {
                //Couldn't find them - most likely invalid GUID
                return Redirect("/");
            }

        }

        public ActionResult RenderDeleteMember(string EmailId)
        {

            //Get member from email
            var Imember = Services.MemberService.GetByEmail(EmailId);
            //Ensure we have that member
            if (Imember != null)
            {
                Services.MemberService.Delete(Imember);
                               

            }

            return Redirect("/approve");
        }


        public ActionResult RenderDownloadCSV(string EmailId)
        {
            if (System.Web.HttpContext.Current.User.IsInRole("shipcenter")
               || System.Web.HttpContext.Current.User.IsInRole("shipdirector") 
               || System.Web.HttpContext.Current.User.IsInRole("shipadmin") )
            {
                List<UserView> users = new List<UserView>();

                var userService = Services.MemberService;


                bool hasloggedin = (System.Web.HttpContext.Current.User != null) &&
        System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
                if (!hasloggedin) return null;

            

                IEnumerable<Umbraco.Core.Models.IMember> userList = null;
                StringBuilder sb = null;

                if (System.Web.HttpContext.Current.User.IsInRole("shipcenter"))
                {
                   userList = userService.GetMembersByPropertyValue("RoleAppliedFor", 7, ValuePropertyMatchType.LessThanOrEqualTo);
                   sb = CreateCSVString(userList);
                }
                if (System.Web.HttpContext.Current.User.IsInRole("shipdirector") || System.Web.HttpContext.Current.User.IsInRole("shipadmin")  )
                {

                                    string usern = string.Empty;
                usern = System.Web.HttpContext.Current.User.Identity.Name;

                var amember = userService.GetByEmail(usern);

                //get the rolid of the  user..
                int intRoleAssigned = int.Parse(amember.GetValue("roleAssigned").ToString());

                string state = string.Empty;
                 state = amember.GetValue("state").ToString();

                 userList = userService.GetMembersByPropertyValue("RoleAppliedFor", intRoleAssigned, ValuePropertyMatchType.LessThanOrEqualTo);

                 var items = userList.Where(x => x.Properties["state"].Value.ToString() == state);
                 sb = CreateCSVString(items);
                }

                         var data = Encoding.UTF8.GetBytes(sb.ToString());
                string filename = "UserList" + ".xls";
                //return File(data, "text/csv", filename);
                return File(data, "application/vnd.ms-excel", filename);
                
            }
            else
            {
                return null;
            }
        }

        private static StringBuilder CreateCSVString(IEnumerable<Umbraco.Core.Models.IMember> userList)
        {
            var sb = new StringBuilder();

            sb.Append("Firstname\t Lastname\t Email\t State\t Role Applied For\t Role Assigned\t Add To Distribution List\t User Status\t Phone\t City\t Organization\t Reason\t  Last Logged in\t Registered date\t Number of logins\t Date approved\t Date denied\t Date inactive\t Last edited\n");


            foreach (var auser in userList)
            {


                string roleNo = auser.GetValue("roleAppliedFor").ToString();


                UiEnum.ShipRolesUmbraco valueEnum = (UiEnum.ShipRolesUmbraco)Enum.Parse(typeof(UiEnum.ShipRolesUmbraco), roleNo, true);


                string roleApplied = valueEnum.Description();

              
               // string roleAssignedNo = (auser.GetValue("roleAssigned") ?? "0").ToString();


                string roleAssigned = string.Empty;
                var roles = System.Web.Security.Roles.GetRolesForUser(auser.Email);
                roleAssigned = string.Join("^", roles);

                //if (String.IsNullOrWhiteSpace(roleAssignedNo))
                //{
                //    roleAssigned = "";
                //}
                //else
                //{
                //    UiEnum.ShipRolesUmbraco valueEnum1 = (UiEnum.ShipRolesUmbraco)Enum.Parse(typeof(UiEnum.ShipRolesUmbraco), roleAssignedNo, true);
                //    roleAssigned = valueEnum1.Description();

                //}

                string _reason = string.Empty;
                if (auser.GetValue("reason") != null)
                    _reason = auser.GetValue("reason").ToString();


               // (myObjc ?? "").ToString()

                string reason = System.Text.RegularExpressions.Regex.Replace(_reason, @"\t|\n|\r", " ");

                sb.Append(string.Format("{0}\t {1}\t {2}\t {3}\t {4}\t {5}\t {6}\t {7}\t {8}\t {9}\t {10}\t {11}\t {12}\t {13}\t {14}\t {15}\t {16}\t{17}\t{18}\n",
                    (auser.GetValue("firstName") ?? "").ToString(),
                    (auser.GetValue("lastName") ?? "").ToString(),
                    auser.Email,
                    (auser.GetValue("state") ?? "").ToString(),
                    roleApplied,
                    roleAssigned,

                    (auser.GetValue("addToDistributionList") ?? "").ToString() == "1",
                    Utils.GetUserStatus(auser),

                    (auser.GetValue("phone") ?? "").ToString(),
                    (auser.GetValue("city") ?? "").ToString(),

                    (auser.GetValue("organization") ?? "").ToString(),
                    reason,
                    (auser.GetValue("lastLoggedIn") ?? "").ToString(),
                    (auser.GetValue("joinedDate") ?? "").ToString(),
                    (auser.GetValue("numberOfLogins") ?? "").ToString(),
                    (auser.GetValue("dateApproved") ?? "").ToString(),
                    (auser.GetValue("dateDenied") ?? "").ToString(),
                    (auser.GetValue("dateInactive") ?? "").ToString(),
                    auser.UpdateDate.ToString()
                    
                    ));

            }
            return sb;
        }
    }
}
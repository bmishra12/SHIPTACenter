using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UmbracoShipTac.Code
{
    public static class Utils
    {

        public static string GetUserStatus(Umbraco.Core.Models.IMember auser)
        {
            //find the status of the user..
            string _status = string.Empty;
            if (auser.GetValue("isDenied").ToString() == "1")
            {
                _status = "Denied";
                return _status;
            }
            if (auser.GetValue("isInactive").ToString() == "1")
            {
                _status = "Inactive";
                return _status;
            }
            if (auser.GetValue("hasVerifiedEmail").ToString() == "0")
            {
                _status = "Pending Email";
                return _status;
            }

            if (auser.IsApproved)
                _status = "Approved";

            else
                _status = "Pending Approval";

            return _status;
        }

        public static string GetUserStatus(string isDenied, string isInactive, string hasVerifiedEmail, string IsApproved)
        {
            //find the status of the user..
            string _status = string.Empty;
            if (isDenied == "1")
            {
                _status = "Denied";
                return _status;
            }
            if (isInactive == "1")
            {
                _status = "Inactive";
                return _status;
            }
            if (hasVerifiedEmail == "0")
            {
                _status = "Pending Email";
                return _status;
            }

            if (IsApproved == "1" || IsApproved == "true")
                _status = "Approved";

            else
                _status = "Pending Approval";

            return _status;
        }


        public static string GetFriendlyRole(string arole)
        {
            //find the status of the user..
            string friendlyRole = string.Empty;
            if (arole == "shiptraining")
            {
                friendlyRole = "SHIP Counselor in training";
                return friendlyRole;
            }
            if (arole == "shipcounselor")
            {
                friendlyRole = "SHIP Counselor";
                return friendlyRole;
            }
            if (arole == "shipstaff")
            {
                friendlyRole = "SHIP Staff";
                return friendlyRole;
            }

            if (arole == "shipadmin")
            {
                friendlyRole = "SHIP Administrator";
                return friendlyRole;
            }
            if (arole == "shipdirector")
            {
                friendlyRole = "SHIP Director";
                return friendlyRole;
            }
            if (arole == "partner")
            {
                friendlyRole = "Partner";
                return friendlyRole;
            }
            if (arole == "acladmin")
            {
                friendlyRole = "ACL Administrator";
                return friendlyRole;
            }
            if (arole == "shipcenter")
            {
                friendlyRole = "SHIP Center";
                return friendlyRole;
            }


            return friendlyRole;
        }
    }
}
using System;
using System.Collections.Generic;

using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Core;

using System.ComponentModel;

namespace UmbracoShipTac.Code
{
    public class UiEnum
    {

        public enum ShipRolesUmbraco
        {
            [Description("No Role Assigned")]
            norole = 0,
            
            [Description("SHIP Counselor in training")]
            shiptraining = 1,

            [Description("SHIP Counselor")]
            shipcounselor = 2,

            [Description("SHIP Staff")]
            shipstaff = 3,

            [Description("SHIP Administrator")]
            shipadmin = 4,

            [Description("SHIP Director")]
            shipdirector = 5,

            [Description("Partner")]
            partner = 6,

            [Description("ACL Administrator")]
            acladmin = 7,

            [Description("SHIP Center")]
            shipcenter = 8
        }


 

    public static List<SelectListItem> RegistrationRoles = new List<SelectListItem>()
    {
        new SelectListItem() {Text="SHIP Counselor in training", Value="shiptraining"},
        new SelectListItem() { Text="SHIP Counselor", Value="shipcounselor"},
        new SelectListItem() { Text="SHIP Staff", Value="shipstaff"},
        new SelectListItem() { Text="SHIP Administrator", Value="shipadmin"},
       new SelectListItem() { Text="Other", Value="shiptrainingother"},

    };


    public static IEnumerable<SelectListItem> GetRegistrationRoles()
    {


            // Have to create new instances via projection
            // to avoid ModelBinding updates to affect this
            // globally

        var mylist = RegistrationRoles
                        .Select(l => new SelectListItem()
                        {
                            Value = l.Value,
                            Text = l.Text
                        });

            return mylist;

      
    }



    public static List<SelectListItem> RolesCanApproveForShipCenter = new List<SelectListItem>()
    {
       new SelectListItem() {Text="SHIP Counselor in training", Value="shiptraining"},
        new SelectListItem() { Text="SHIP Counselor", Value="shipcounselor"},
        new SelectListItem() { Text="SHIP Staff", Value="shipstaff"},
        new SelectListItem() { Text="SHIP Administrator", Value="shipadmin"},
        new SelectListItem() { Text="SHIP Director", Value="shipdirector"},
        new SelectListItem() { Text="Partner", Value="partner"},
        new SelectListItem() { Text="ACL Administrator", Value="acladmin"},


    };

    public static List<SelectListItem> RolesCanApproveForAclAdmin = new List<SelectListItem>()
    {
       new SelectListItem() {Text="SHIP Counselor in training", Value="shiptraining"},
        new SelectListItem() { Text="SHIP Counselor", Value="shipcounselor"},
        new SelectListItem() { Text="SHIP Staff", Value="shipstaff"},
        new SelectListItem() { Text="SHIP Administrator", Value="shipadmin"},
        new SelectListItem() { Text="SHIP Director", Value="shipdirector"},
          new SelectListItem() { Text="Partner", Value="partner"},


    };

    public static List<SelectListItem> RolesCanApproveForShipDirector = new List<SelectListItem>()
    {
       new SelectListItem() {Text="SHIP Counselor in training", Value="shiptraining"},
        new SelectListItem() { Text="SHIP Counselor", Value="shipcounselor"},
        new SelectListItem() { Text="SHIP Staff", Value="shipstaff"},
        new SelectListItem() { Text="SHIP Administrator", Value="shipadmin"},


    };

    public static List<SelectListItem> RolesCanApproveForShipAdmin = new List<SelectListItem>()
    {
       new SelectListItem() {Text="SHIP Counselor in training", Value="shiptraining"},
        new SelectListItem() { Text="SHIP Counselor", Value="shipcounselor"},
        new SelectListItem() { Text="SHIP Staff", Value="shipstaff"},

    };

    public static IEnumerable<SelectListItem> GetRolesCanApproveForAdmins(bool isAddUser)
    {

        if (System.Web.HttpContext.Current.User.IsInRole("shipcenter"))
        {
                 // Have to create new instances via projection
                // to avoid ModelBinding updates to affect this
                // globally

            //changed the logic to show all the users that shipcenter can access..
            //the isAddUser is not used now..  The old logic was while updating show lesser access which is not valid any more..
                var mylist = RolesCanApproveForShipCenter
                            .Select(l => new SelectListItem()
                            {
                                Value = l.Value,
                                Text = l.Text
                            });

                return mylist;
               
            
        }
        else if (System.Web.HttpContext.Current.User.IsInRole("acladmin"))
        {
            var mylist = RolesCanApproveForAclAdmin
            .Select(l => new SelectListItem()
            {
                Value = l.Value,
                Text = l.Text
            });

            return mylist;
            
        }
        else if (System.Web.HttpContext.Current.User.IsInRole("shipdirector"))
        {
            var mylist = RolesCanApproveForShipDirector
            .Select(l => new SelectListItem()
            {
                Value = l.Value,
                Text = l.Text
            });

            return mylist;

        }
        else if (System.Web.HttpContext.Current.User.IsInRole("shipadmin"))
        {
                    var mylist = RolesCanApproveForShipAdmin
            .Select(l => new SelectListItem()
            {
                Value = l.Value,
                Text = l.Text
            });

            return mylist;

        }
        else
            return null;
    }

    public static IEnumerable<SelectListItem> GetAclStates()
    {
        // Have to create new instances via projection
        // to avoid ModelBinding updates to affect this
        // globally

        var mylist = AclStates
                        .Select(l => new SelectListItem()
                        {
                            Value = l.Value,
                            Text = l.Text
                        });

        return mylist;

    }

    public static IEnumerable<SelectListItem> GetAclStates1()
    {
            // Have to create new instances via projection
            // to avoid ModelBinding updates to affect this
            // globally

        var mylist = AclStates1
                        .Select(l => new SelectListItem()
                        {
                            Value = l.Value,
                            Text = l.Text
                        });

            return mylist;

    }

    public static IEnumerable<SelectListItem> GetStates()
    {
        // Have to create new instances via projection
        // to avoid ModelBinding updates to affect this
        // globally

        var mylist = States
                        .Select(l => new SelectListItem()
                        {
                            Value = l.Value,
                            Text = l.Text
                        });

        return mylist;

    }

    public static List<SelectListItem> States = new List<SelectListItem>()
    {
        new SelectListItem() {Text="Alabama", Value="AL"},
        new SelectListItem() { Text="Alaska", Value="AK"},
        new SelectListItem() { Text="Arizona", Value="AZ"},
        new SelectListItem() { Text="Arkansas", Value="AR"},
        new SelectListItem() { Text="California", Value="CA"},
        new SelectListItem() { Text="Colorado", Value="CO"},
        new SelectListItem() { Text="Connecticut", Value="CT"},
        new SelectListItem() { Text="District of Columbia", Value="DC"},
        new SelectListItem() { Text="Delaware", Value="DE"},
        new SelectListItem() { Text="Florida", Value="FL"},
        new SelectListItem() { Text="Georgia", Value="GA"},
        new SelectListItem() { Text="Hawaii", Value="HI"},
        new SelectListItem() { Text="Idaho", Value="ID"},
        new SelectListItem() { Text="Illinois", Value="IL"},
        new SelectListItem() { Text="Indiana", Value="IND"},
        new SelectListItem() { Text="Iowa", Value="IA"},
        new SelectListItem() { Text="Kansas", Value="KS"},
        new SelectListItem() { Text="Kentucky", Value="KY"},
        new SelectListItem() { Text="Louisiana", Value="LA"},
        new SelectListItem() { Text="Maine", Value="ME"},
        new SelectListItem() { Text="Maryland", Value="MD"},
        new SelectListItem() { Text="Massachusetts", Value="MA"},
        new SelectListItem() { Text="Michigan", Value="MI"},
        new SelectListItem() { Text="Minnesota", Value="MN"},
        new SelectListItem() { Text="Mississippi", Value="MS"},
        new SelectListItem() { Text="Missouri", Value="MO"},
        new SelectListItem() { Text="Montana", Value="MT"},
        new SelectListItem() { Text="Nebraska", Value="NE"},
        new SelectListItem() { Text="Nevada", Value="NV"},
        new SelectListItem() { Text="New Hampshire", Value="NH"},
        new SelectListItem() { Text="New Jersey", Value="NJ"},
        new SelectListItem() { Text="New Mexico", Value="NM"},
        new SelectListItem() { Text="New York", Value="NY"},
        new SelectListItem() { Text="North Carolina", Value="NC"},
        new SelectListItem() { Text="North Dakota", Value="ND"},
        new SelectListItem() { Text="Ohio", Value="OH"},
        new SelectListItem() { Text="Oklahoma", Value="OK"},
        new SelectListItem() { Text="Oregon", Value="ORE"},
        new SelectListItem() { Text="Pennsylvania", Value="PA"},
        new SelectListItem() { Text="Rhode Island", Value="RI"},
        new SelectListItem() { Text="South Carolina", Value="SC"},
        new SelectListItem() { Text="South Dakota", Value="SD"},
        new SelectListItem() { Text="Tennessee", Value="TN"},
        new SelectListItem() { Text="Texas", Value="TX"},
        new SelectListItem() { Text="Utah", Value="UT"},
        new SelectListItem() { Text="Vermont", Value="VT"},
        new SelectListItem() { Text="Virginia", Value="VA"},
        new SelectListItem() { Text="Washington", Value="WA"},
        new SelectListItem() { Text="West Virginia", Value="WV"},
        new SelectListItem() { Text="Wisconsin", Value="WI"},
        new SelectListItem() { Text="Wyoming", Value="WY"},
            new SelectListItem() { Text="Guam", Value="GU"},
            new SelectListItem() { Text="Puerto Rico", Value="PR"},
            new SelectListItem() { Text="Virgin Islands of the US", Value="VI"},


    
    };

        public static List<SelectListItem> AclStates = new List<SelectListItem>()
    {
        new SelectListItem() {Text="Alabama", Value="AL"},
        new SelectListItem() { Text="Alaska", Value="AK"},
        new SelectListItem() { Text="Arizona", Value="AZ"},
        new SelectListItem() { Text="Arkansas", Value="AR"},
        new SelectListItem() { Text="California", Value="CA"},
        new SelectListItem() { Text="Colorado", Value="CO"},
        new SelectListItem() { Text="Connecticut", Value="CT"},
        new SelectListItem() { Text="District of Columbia", Value="DC"},
        new SelectListItem() { Text="Delaware", Value="DE"},
        new SelectListItem() { Text="Florida", Value="FL"},
        new SelectListItem() { Text="Georgia", Value="GA"},
        new SelectListItem() { Text="Hawaii", Value="HI"},
        new SelectListItem() { Text="Idaho", Value="ID"},
        new SelectListItem() { Text="Illinois", Value="IL"},
        new SelectListItem() { Text="Indiana", Value="IND"},
        new SelectListItem() { Text="Iowa", Value="IA"},
        new SelectListItem() { Text="Kansas", Value="KS"},
        new SelectListItem() { Text="Kentucky", Value="KY"},
        new SelectListItem() { Text="Louisiana", Value="LA"},
        new SelectListItem() { Text="Maine", Value="ME"},
        new SelectListItem() { Text="Maryland", Value="MD"},
        new SelectListItem() { Text="Massachusetts", Value="MA"},
        new SelectListItem() { Text="Michigan", Value="MI"},
        new SelectListItem() { Text="Minnesota", Value="MN"},
        new SelectListItem() { Text="Mississippi", Value="MS"},
        new SelectListItem() { Text="Missouri", Value="MO"},
        new SelectListItem() { Text="Montana", Value="MT"},
        new SelectListItem() { Text="Nebraska", Value="NE"},
        new SelectListItem() { Text="Nevada", Value="NV"},
        new SelectListItem() { Text="New Hampshire", Value="NH"},
        new SelectListItem() { Text="New Jersey", Value="NJ"},
        new SelectListItem() { Text="New Mexico", Value="NM"},
        new SelectListItem() { Text="New York", Value="NY"},
        new SelectListItem() { Text="North Carolina", Value="NC"},
        new SelectListItem() { Text="North Dakota", Value="ND"},
        new SelectListItem() { Text="Ohio", Value="OH"},
        new SelectListItem() { Text="Oklahoma", Value="OK"},
        new SelectListItem() { Text="Oregon", Value="ORE"},
        new SelectListItem() { Text="Pennsylvania", Value="PA"},
        new SelectListItem() { Text="Rhode Island", Value="RI"},
        new SelectListItem() { Text="South Carolina", Value="SC"},
        new SelectListItem() { Text="South Dakota", Value="SD"},
        new SelectListItem() { Text="Tennessee", Value="TN"},
        new SelectListItem() { Text="Texas", Value="TX"},
        new SelectListItem() { Text="Utah", Value="UT"},
        new SelectListItem() { Text="Vermont", Value="VT"},
        new SelectListItem() { Text="Virginia", Value="VA"},
        new SelectListItem() { Text="Washington", Value="WA"},
        new SelectListItem() { Text="West Virginia", Value="WV"},
        new SelectListItem() { Text="Wisconsin", Value="WI"},
        new SelectListItem() { Text="Wyoming", Value="WY"},

            new SelectListItem() { Text="Guam", Value="GU"},
            new SelectListItem() { Text="Puerto Rico", Value="PR"},
            new SelectListItem() { Text="Virgin Islands of the US", Value="VI"},

                new SelectListItem() { Text="ACL", Value="ACL"},
                new SelectListItem() { Text="PARTNER", Value="PARTNER"},
                new SelectListItem() { Text="CENTER", Value="CENTER"}

    
    };

        public static List<SelectListItem> AclStates1 = new List<SelectListItem>()
    {
        //Guam comes after Georgia, Puerto Rico is after Pennsylvania..etc 
                new SelectListItem() { Text="ACL", Value="ACL"},
                new SelectListItem() { Text="CENTER", Value="CENTER"},
                new SelectListItem() { Text="PARTNER", Value="PARTNER"},

        new SelectListItem() {Text="Alabama", Value="AL"},
        new SelectListItem() { Text="Alaska", Value="AK"},
        new SelectListItem() { Text="Arizona", Value="AZ"},
        new SelectListItem() { Text="Arkansas", Value="AR"},
        new SelectListItem() { Text="California", Value="CA"},
        new SelectListItem() { Text="Colorado", Value="CO"},
        new SelectListItem() { Text="Connecticut", Value="CT"},
        new SelectListItem() { Text="District of Columbia", Value="DC"},
        new SelectListItem() { Text="Delaware", Value="DE"},
        new SelectListItem() { Text="Florida", Value="FL"},
        new SelectListItem() { Text="Georgia", Value="GA"},
            new SelectListItem() { Text="Guam", Value="GU"},

        new SelectListItem() { Text="Hawaii", Value="HI"},
        new SelectListItem() { Text="Idaho", Value="ID"},
        new SelectListItem() { Text="Illinois", Value="IL"},
        new SelectListItem() { Text="Indiana", Value="IND"},
        new SelectListItem() { Text="Iowa", Value="IA"},
        new SelectListItem() { Text="Kansas", Value="KS"},
        new SelectListItem() { Text="Kentucky", Value="KY"},
        new SelectListItem() { Text="Louisiana", Value="LA"},
        new SelectListItem() { Text="Maine", Value="ME"},
        new SelectListItem() { Text="Maryland", Value="MD"},
        new SelectListItem() { Text="Massachusetts", Value="MA"},
        new SelectListItem() { Text="Michigan", Value="MI"},
        new SelectListItem() { Text="Minnesota", Value="MN"},
        new SelectListItem() { Text="Mississippi", Value="MS"},
        new SelectListItem() { Text="Missouri", Value="MO"},
        new SelectListItem() { Text="Montana", Value="MT"},
        new SelectListItem() { Text="Nebraska", Value="NE"},
        new SelectListItem() { Text="Nevada", Value="NV"},
        new SelectListItem() { Text="New Hampshire", Value="NH"},
        new SelectListItem() { Text="New Jersey", Value="NJ"},
        new SelectListItem() { Text="New Mexico", Value="NM"},
        new SelectListItem() { Text="New York", Value="NY"},
        new SelectListItem() { Text="North Carolina", Value="NC"},
        new SelectListItem() { Text="North Dakota", Value="ND"},
        new SelectListItem() { Text="Ohio", Value="OH"},
        new SelectListItem() { Text="Oklahoma", Value="OK"},
        new SelectListItem() { Text="Oregon", Value="ORE"},
        new SelectListItem() { Text="Pennsylvania", Value="PA"},

                    new SelectListItem() { Text="Puerto Rico", Value="PR"},

        new SelectListItem() { Text="Rhode Island", Value="RI"},
        new SelectListItem() { Text="South Carolina", Value="SC"},
        new SelectListItem() { Text="South Dakota", Value="SD"},
        new SelectListItem() { Text="Tennessee", Value="TN"},
        new SelectListItem() { Text="Texas", Value="TX"},
        new SelectListItem() { Text="Utah", Value="UT"},
        new SelectListItem() { Text="Vermont", Value="VT"},
        new SelectListItem() { Text="Virginia", Value="VA"},
                    new SelectListItem() { Text="Virgin Islands of the US", Value="VI"},

        new SelectListItem() { Text="Washington", Value="WA"},
        new SelectListItem() { Text="West Virginia", Value="WV"},
        new SelectListItem() { Text="Wisconsin", Value="WI"},
        new SelectListItem() { Text="Wyoming", Value="WY"}

    
    };

        public static List<SelectListItem> AclStatesALL = new List<SelectListItem>()
    {
        new SelectListItem() {Text="Alabama", Value="AL"},
        new SelectListItem() { Text="Alaska", Value="AK"},
        new SelectListItem() { Text="Arizona", Value="AZ"},
        new SelectListItem() { Text="Arkansas", Value="AR"},
        new SelectListItem() { Text="California", Value="CA"},
        new SelectListItem() { Text="Colorado", Value="CO"},
        new SelectListItem() { Text="Connecticut", Value="CT"},
        new SelectListItem() { Text="District of Columbia", Value="DC"},
        new SelectListItem() { Text="Delaware", Value="DE"},
        new SelectListItem() { Text="Florida", Value="FL"},
        new SelectListItem() { Text="Georgia", Value="GA"},
        new SelectListItem() { Text="Hawaii", Value="HI"},
        new SelectListItem() { Text="Idaho", Value="ID"},
        new SelectListItem() { Text="Illinois", Value="IL"},
        new SelectListItem() { Text="Indiana", Value="IND"},
        new SelectListItem() { Text="Iowa", Value="IA"},
        new SelectListItem() { Text="Kansas", Value="KS"},
        new SelectListItem() { Text="Kentucky", Value="KY"},
        new SelectListItem() { Text="Louisiana", Value="LA"},
        new SelectListItem() { Text="Maine", Value="ME"},
        new SelectListItem() { Text="Maryland", Value="MD"},
        new SelectListItem() { Text="Massachusetts", Value="MA"},
        new SelectListItem() { Text="Michigan", Value="MI"},
        new SelectListItem() { Text="Minnesota", Value="MN"},
        new SelectListItem() { Text="Mississippi", Value="MS"},
        new SelectListItem() { Text="Missouri", Value="MO"},
        new SelectListItem() { Text="Montana", Value="MT"},
        new SelectListItem() { Text="Nebraska", Value="NE"},
        new SelectListItem() { Text="Nevada", Value="NV"},
        new SelectListItem() { Text="New Hampshire", Value="NH"},
        new SelectListItem() { Text="New Jersey", Value="NJ"},
        new SelectListItem() { Text="New Mexico", Value="NM"},
        new SelectListItem() { Text="New York", Value="NY"},
        new SelectListItem() { Text="North Carolina", Value="NC"},
        new SelectListItem() { Text="North Dakota", Value="ND"},
        new SelectListItem() { Text="Ohio", Value="OH"},
        new SelectListItem() { Text="Oklahoma", Value="OK"},
        new SelectListItem() { Text="Oregon", Value="ORE"},
        new SelectListItem() { Text="Pennsylvania", Value="PA"},
        new SelectListItem() { Text="Rhode Island", Value="RI"},
        new SelectListItem() { Text="South Carolina", Value="SC"},
        new SelectListItem() { Text="South Dakota", Value="SD"},
        new SelectListItem() { Text="Tennessee", Value="TN"},
        new SelectListItem() { Text="Texas", Value="TX"},
        new SelectListItem() { Text="Utah", Value="UT"},
        new SelectListItem() { Text="Vermont", Value="VT"},
        new SelectListItem() { Text="Virginia", Value="VA"},
        new SelectListItem() { Text="Washington", Value="WA"},
        new SelectListItem() { Text="West Virginia", Value="WV"},
        new SelectListItem() { Text="Wisconsin", Value="WI"},
        new SelectListItem() { Text="Wyoming", Value="WY"},

            new SelectListItem() { Text="Guam", Value="GU"},
            new SelectListItem() { Text="Puerto Rico", Value="PR"},
            new SelectListItem() { Text="Virgin Islands of the US", Value="VI"},

                new SelectListItem() { Text="ACL", Value="ACL"},
                new SelectListItem() { Text="PARTNER", Value="PARTNER"},
                new SelectListItem() { Text="CENTER", Value="CENTER"},
                new SelectListItem() { Text="ALL", Value="ALL"}

    
    };




     public static List<SelectListItem> ResourceRoles = new List<SelectListItem>()
        {
            new SelectListItem() { Text="SHIP counselors and higher", Value="1"},
            new SelectListItem() { Text="SHIP staff and higher", Value="2"},
            new SelectListItem() { Text="SHIP administrators and higher", Value="3"},
            new SelectListItem() { Text="SHIP directors and higher", Value="4"},
 
        };

     public static List<SelectListItem> ResourceRolesForShipAdmin = new List<SelectListItem>()
        {
            new SelectListItem() { Text="SHIP counselors and higher", Value="1"},
            new SelectListItem() { Text="SHIP staff and higher", Value="2"},
            new SelectListItem() { Text="SHIP administrators and higher", Value="3"},
 
        };

     public static List<SelectListItem> ResourceFileTypes = new List<SelectListItem>()
        {
            new SelectListItem() { Text="Advertisement", Value="Advertisement"},
            new SelectListItem() { Text="Brochure", Value="Brochure"},
            new SelectListItem() { Text="Checklist", Value="Checklist"},
            new SelectListItem() { Text="Consumer Guide", Value="ConsumerGuide"},
            new SelectListItem() { Text="Contract/Agreement", Value="Contract/Agreement"},
            new SelectListItem() { Text="Handout", Value="Handout"},
            new SelectListItem() { Text="Letter", Value="Letter"},
            new SelectListItem() { Text="Manual/User Guide", Value="Manual/User Guide"},
            new SelectListItem() { Text="Policy/protocol", Value="Policy/protocol"},
            new SelectListItem() { Text="PowerPoint", Value="PowerPoint"},
            new SelectListItem() { Text="Report", Value="Report"},
            new SelectListItem() { Text="Script", Value="Script"},
            new SelectListItem() { Text="Social Media", Value="Social Media"},
            new SelectListItem() { Text="Toolkit", Value="Toolkit"},
            new SelectListItem() { Text="Video", Value="Video"},
          new SelectListItem() { Text="Webinar", Value="Webinar"},
          new SelectListItem() { Text="Website", Value="Website"},
        };

    }

}
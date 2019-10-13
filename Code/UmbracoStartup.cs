using System.Web.Mvc;
using System.Web.Routing;
using Umbraco.Core;

namespace UmbracoShipTac.Code
{
    public class UmbracoStartup : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            //Register custom MVC route for user profile
            RegisterRoutes(RouteTable.Routes);
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            //routes.MapRoute(
            //    "MemberProfile",                                                        // Route name
            //    "user/{profileURLtoCheck}",                                             // URL with parameters
            //    new { controller = "ProfileSurface", action = "RenderMemberProfile" }   // Parameter defaults
            //);

            routes.MapRoute(
                "MemberRegister",                                                        // Route name
                "customroute/{action}/{id}",                                             // URL with parameters
                new
                {
                    controller = "ShipTac",
                    action = "RenderVerifyEmail",
                    verifyGUID = string.Empty
                }   // Parameter defaults
            );

            routes.MapRoute(
                "MemberApprove",                                                        // Route name
                "customroute/{action}/{id}",                                             // URL with parameters
                new
                {
                    controller = "ShipTac",
                    action = "RenderApproveMember",
                    EmailId = string.Empty
                }   // Parameter defaults
            );


            routes.MapRoute(
    "DownloadCSV",                                                        // Route name
    "customroute/{action}/{id}",                                             // URL with parameters
    new
    {
        controller = "ShipTac",
        action = "RenderDownloadCSV",
        EmailId = string.Empty
    }   // Parameter defaults
);

            routes.MapRoute(
                "MemberDeny",                                                        // Route name
                "customroute/{action}/{id}",                                             // URL with parameters
                new
                {
                    controller = "ShipTac",
                    action = "RenderDenyMember",
                    EmailId = string.Empty
                }   // Parameter defaults
            );


            routes.MapRoute(
                "MemberDelete",                                                        // Route name
                "customroute/{action}/{id}",                                             // URL with parameters
                new
                {
                    controller = "ShipTac",
                    action = "RenderDeleteMember",
                    EmailId = string.Empty
                }   // Parameter defaults
            );



            //RouteTable.Routes.RouteExistingFiles = true;
            ////Create a custom routes
            //RouteTable.Routes.MapRoute(
            //    "",
            //    "Media/{id}/{file}",
            //    new
            //    {
            //        controller = "Media",
            //        action = "Index",
            //        id = UrlParameter.Optional,
            //        file = UrlParameter.Optional
            //    });

        }
    }
}
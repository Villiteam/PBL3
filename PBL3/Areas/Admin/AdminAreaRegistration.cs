using System.Web.Mvc;

namespace PBL3.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "PBL3.Areas.Admin.Controllers" }
            );

            context.MapRoute(
               name: "Admin",
               url: "admin",
               defaults: new { controller = "AdminHome", action = "Index", id = UrlParameter.Optional },
               namespaces: new[] { "PBL3.Areas.Admin.Controllers" }
           );
        }
    }
}
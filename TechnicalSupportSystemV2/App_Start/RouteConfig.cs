using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TechnicalSupportSystem
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Student", "Student/{action}/{id}",
            new { controller = "Student", action = "Index", id = UrlParameter.Optional });

            routes.MapRoute("Supervisor", "Supervisor/{action}/{id}",
            new { controller = "Supervisor", action = "Index", id = UrlParameter.Optional });

            routes.MapRoute("Technician", "Technician/{action}/{id}",
            new { controller = "Technician", action = "Index", id = UrlParameter.Optional });

            routes.MapRoute("Admin", "Admin/{action}/{id}",
            new { controller = "Admin", action = "Index", id = UrlParameter.Optional });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
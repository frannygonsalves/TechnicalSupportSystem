using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using TechnicalSupportSystem.Models;
using TechnicalSupportSystemV2.DAL;
using TechnicalSupportSystemV2.Infrastructure;
using TechnicalSupportSystemV2.Models;
using WebMatrix.WebData;

namespace TechnicalSupportSystemV2
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            DependencyResolver.SetResolver(new NinjectDependencyResolver());

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            AuthConfig.RegisterAuth();

            WebSecurity.InitializeDatabaseConnection("SystemDBContext", "UserProfile", "UserID", "UserName", autoCreateTables: true);

            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //BootstrapSupport.BootstrapBundleConfig.RegisterBundles(System.Web.Optimization.BundleTable.Bundles);
        }
    }
}
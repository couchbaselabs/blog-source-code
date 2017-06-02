using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Couchbase;
using Couchbase.Authentication;
using Couchbase.Configuration.Client;

namespace loggingsample
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            log4net.GlobalContext.Properties["LogFileLocation"] = Path.Combine(Server.MapPath("~/Logs"), "LogFile068.txt");
            log4net.Config.XmlConfigurator.Configure();

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ClusterHelper.Initialize(new ClientConfiguration
            {
                Servers = new List<Uri> {  new Uri("http://localhost:8091")}
            });
            var cluster = ClusterHelper.Get();
            var creds = new PasswordAuthenticator("myuser", "password");
            cluster.Authenticate(creds);
        }
    }
}

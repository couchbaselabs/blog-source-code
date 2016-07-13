using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Couchbase;
using Couchbase.Configuration.Client;

namespace CouchbaseAspNetExample3
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            var config = new ClientConfiguration();
            config.Servers = new List<Uri>
            {
                new Uri("http://localhost:8091")
            };
            config.UseSsl = false;
            ClusterHelper.Initialize(config);
        }

        protected void Application_End()
        {
            ClusterHelper.Close();
        }
    }
}

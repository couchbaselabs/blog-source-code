using System;
using System.Collections.Generic;
// tag::namespaces[]
using Couchbase.Extensions.Caching;
using Couchbase.Extensions.DependencyInjection;
using Couchbase.Extensions.Session;
// end::namespaces[]
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AspNetDistributedSession
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            /* this will setup the standard in-process session
           // tag::ConfigureSession[]
           services.AddDistributedMemoryCache();
           services.AddSession();
           // end::ConfigureSession[]
           */

            // tag::CouchbaseConfig[]
            services.AddCouchbase(opt =>
            {
                opt.Servers = new List<Uri> { new Uri("http://localhost:8091") };
            });

            services.AddDistributedCouchbaseCache("sessionstore", "password", opt => { });
            // end::CouchbaseConfig[]

            // tag::CouchbaseSession[]
            services.AddCouchbaseSession(opt =>
            {
                opt.CookieName = ".MyApp.Cookie";
                opt.IdleTimeout = new TimeSpan(0, 0, 20, 0);
            });
            // end::CouchbaseSession[]
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime appLifetime)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            // this will tell ASP.NET Core to use session
            // tag::UseSession[]
            app.UseSession();
            // end::UseSession[]

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // tag::cleanup[]
            appLifetime.ApplicationStopped.Register(() =>
            {
                app.ApplicationServices.GetRequiredService<ICouchbaseLifetimeService>().Close();
            });
            // end::cleanup[]
        }
    }
}

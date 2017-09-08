using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Couchbase.Extensions.DependencyInjection;
using CouchbaseDIExample.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CouchbaseDIExample
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
            // Add framework services.
            services.AddMvc();

            /*
            // you can hardcode the config
            // tag::noconfig[]
            services.AddCouchbase(client =>
            {
                client.Servers = new List<Uri> { new Uri("http://localhost:8091")};
                client.UseSsl = false;
            });
            // end::noconfig[]
            */

            /*
            // tag::config[]
            services.AddCouchbase(Configuration.GetSection("Couchbase"));
            // end::config[]
            */

            // tag::namedbucket[]
            services
                .AddCouchbase(Configuration.GetSection("Couchbase"))
                .AddCouchbaseBucket<ITravelSampleBucketProvider>("travel-sample", "password");
            // end::namedbucket[]

            // tag::moredi[]
            services.AddTransient<IEmailService, MyEmailService>();
            services.AddTransient<IComplexService, ComplexService>();
            // end::moredi[]
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // tag::Configure[]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime applicationLifetime)
        // end::Configure[]
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

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // tag::cleanup[]
            applicationLifetime.ApplicationStopped.Register(() =>
            {
                app.ApplicationServices.GetRequiredService<ICouchbaseLifetimeService>().Close();
            });
            // end::cleanup[]
        }
    }
}

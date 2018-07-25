using System;
using System.Collections.Generic;
using Couchbase.Extensions.Caching;
using Couchbase.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DistributedCachingExample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCaching();

            services.AddMvc();

            // tag::AddCouchbase[]
            services.AddCouchbase(opt =>
            {
                opt.Servers = new List<Uri>
                {
                    new Uri("http://localhost:8091")
                };
                opt.Username = "infoq";
                opt.Password = "password";
            });

            services.AddDistributedCouchbaseCache("infoqcache", opt => { });
            // end::AddCouchbase[]
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}

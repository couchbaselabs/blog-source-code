using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.Core;
using Couchbase.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AspNetDnsDiscovery.Controllers
{
    public class HomeController : Controller
    {
        readonly IBucket _bucket;

        public HomeController(IBucketProvider bucketProvider)
        {
            _bucket = bucketProvider.GetBucket("default","password");
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            var result = _bucket.Upsert(new Document<dynamic>
            {
                Id = "mydemodoc",
                Content = new
                {
                    Foo = "bar",
                    Baz = Guid.NewGuid()
                }
            });

            ViewData["Message"] = $"Upsert successful: {result.Success}"; // + JsonConvert.SerializeObject(_bucket.Configuration);

            ViewData["Servers"] = string.Join(",",_bucket.Configuration.Servers.Select(s => s.ToString()));

            //HttpContext.Response.WriteAsync("hey"); //JsonConvert.SerializeObject(_bucket.Configuration));

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Couchbase.Extensions.Caching;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace CouchbaseAspNetCaching.Controllers
{
    public class HomeController : Controller
    {
        private IDistributedCache _cache;

        public HomeController(IDistributedCache cache)
        {
            _cache = cache;
        }

        public IActionResult Index()
        {
            return View();
        }

        public class Wingwang
        {
            public string Wahoo { get; set; }
        }

        public IActionResult About()
        {
            var message = _cache.GetString("warbl");
            if (message == null)
            {
                message = DateTime.Now + " " + Path.GetRandomFileName();
                _cache.SetString("warbl", message);
            }
            _cache.Set<Wingwang>(Path.GetRandomFileName(), new Wingwang { Wahoo = "flurtnurt"},null);

            ViewData["Message"] = message;
            

//            ViewData["Message"] = DateTime.Now + " " + Path.GetRandomFileName();

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

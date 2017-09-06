using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Couchbase.Extensions.Caching;
using CouchbaseAspNetCaching.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace CouchbaseAspNetCaching.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDistributedCache _cache;

        public HomeController(IDistributedCache cache)
        {
            _cache = cache;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            // cache/retrieve from cache
            // a string, stored under key "CachedString1"
            var message = _cache.GetString("CachedString1");
            if (message == null)
            {
                message = DateTime.Now + " " + Path.GetRandomFileName();
                _cache.SetString("CachedString1", message);
            }
            ViewData["Message"] = "'CachedString1' is '" + message + "'";

            // cache a generated POCO
            // store under a random key
            var pocoKey = Path.GetRandomFileName();
            _cache.Set(pocoKey, MyPoco.Generate(), null);
            ViewData["Message2"] = "Cached a POCO in '" + pocoKey + "'";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Hello world " + Path.GetRandomFileName();

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}

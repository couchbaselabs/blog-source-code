using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Couchbase.Core;
using Couchbase.Extensions.DependencyInjection;
using CouchbaseDIExample.Models;
using Microsoft.AspNetCore.Mvc;

namespace CouchbaseDIExample.Controllers
{
    // tag::inject[]
    public class HomeController : Controller
    {
        private readonly IBucket _bucket;

        public HomeController(IBucketProvider bucketProvider)
        {
            _bucket = bucketProvider.GetBucket("travel-sample", "password");
        }
        // end::inject[]

        /*
        // tag::bucketinject[]
        public HomeController(ITravelSampleBucketProvider travelBucketProvider)
        {
            _bucket = travelBucketProvider.GetBucket();
        }
        // end::bucketinject[]
        */

        public IActionResult Index()
        {
            return View();
        }

        // just to show that the bucket is being injected properly
        // tag::About[]
        public IActionResult About()
        {
            // get the route document for Columbus to Chicago (United)
            var route = _bucket.Get<dynamic>("route_56027").Value;

            // display the equipment number of the route
            ViewData["Message"] = "CMH to ORD - " + route.equipment;

            return View();
        }
        // end::About[]

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

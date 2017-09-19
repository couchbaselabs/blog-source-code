using System;
using Couchbase.Extensions.Session;
using Microsoft.AspNetCore.Mvc;

namespace AspNetDistributedSession.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // tag::Another[]
            HttpContext.Session.SetObject("sessionkey2", new
            {
                Address = "123 Main St",
                City = "Lancaster",
                State = "OH"
            });
            // tag::Another[]

            return View();
        }

        // tag::About[]
        public IActionResult About()
        {
            HttpContext.Session.SetObject("sessionkey", new
            {
                Name = "Matt",
                Twitter = "@mgroves",
                Guid = DateTime.Now
            });

            ViewData["Message"] = "I put a value in your session. Click 'Contact' to see it.";

            return View();
        }
        // end::About[]

        // tag::Contact[]
        public IActionResult Contact()
        {
            ViewData["Message"] = HttpContext.Session.GetObject<dynamic>("sessionkey");

            return View();
        }
        // end::Contact[]

        public IActionResult Error()
        {
            return View();
        }
    }
}

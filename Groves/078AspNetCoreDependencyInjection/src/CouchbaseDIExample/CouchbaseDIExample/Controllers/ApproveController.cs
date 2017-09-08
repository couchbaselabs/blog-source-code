using CouchbaseDIExample.Models;
using Microsoft.AspNetCore.Mvc;

namespace CouchbaseDIExample.Controllers
{
    // tag::ApproveController[]
    public class ApproveController : Controller
    {
        private readonly IComplexService _svc;

        public ApproveController(IComplexService svc)
        {
            _svc = svc;
        }

        public IActionResult Index()
        {
            var fakeEmailAddress = Faker.Internet.Email();
            _svc.ApproveApplication(fakeEmailAddress);
            ViewData["Message"] = "Approved '" + fakeEmailAddress + "'";
            return View();
        }
    }
    // end::ApproveController[]
}

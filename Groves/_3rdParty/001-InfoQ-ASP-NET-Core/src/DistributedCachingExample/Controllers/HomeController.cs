using Microsoft.AspNetCore.Mvc;

namespace DistributedCachingExample.Controllers
{
    public class HomeController : Controller
    {
        [Route("/")]
        public IActionResult Index()
        {
            return View();
        }
//
//        [Route("/test2")]
//        [ResponseCache(Duration = 3600)]
//        public IActionResult FullPageCache()
//        {
//            return View();
//        }
    }
}
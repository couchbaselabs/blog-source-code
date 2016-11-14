using DockerNetCore.Model;
using Microsoft.AspNetCore.Mvc;

namespace DockerNetCore.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var gifts = Gift.GetAllGifts();
            return View(gifts);
        }

        public IActionResult AddGift()
        {
            Gift.CreateNewGift();

            return RedirectToAction("Index");
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}

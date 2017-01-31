using System.Web.Mvc;

namespace SQLServerToCouchbase.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.DatabaseType = MvcApplication.UseSQL ? "SQL Server" : "Couchbase Server";
            return View();
        }
    }
}
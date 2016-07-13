using System.Web.Mvc;
using System.Web.UI.WebControls;
using Couchbase;
using Couchbase.Core;

namespace CouchbaseAspNetExample3.Controllers
{
    public class TestController : Controller
    {
        readonly IBucket _bucket;

        public TestController()
        {
            _bucket = ClusterHelper.GetBucket("default");
        }

        public ActionResult Index()
        {
            var doc = _bucket.Get<dynamic>("foo::123");
            return Content("Name: " + doc.Value.name + ", Address: " + doc.Value.address);
        }
    }
}
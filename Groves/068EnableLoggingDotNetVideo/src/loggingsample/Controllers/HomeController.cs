using System;
using System.IO;
using System.Text;
using System.Web.Mvc;
using Couchbase;
using Couchbase.Core;
using Couchbase.N1QL;
using loggingsample.Models;

namespace loggingsample.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBucket _bucket;

        public HomeController()
        {
            _bucket = ClusterHelper.GetBucket("mybucket");
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult N1qlExample()
        {
            var query = QueryRequest.Create("SELECT b.foo, b.bar FROM `" + _bucket.Name + "` b");
            query.ScanConsistency(ScanConsistency.RequestPlus);
            var results = _bucket.Query<FooBar>(query).Rows;

            return View(results);
        }

        public ActionResult KeyValueExample()
        {
            var key = Guid.NewGuid().ToString();
            _bucket.Insert(key, FooBar.Generate());
            var foobar = _bucket.Get<FooBar>(key).Value;
            ViewBag.Key = key;
            return View(foobar);
        }

        public ActionResult ViewLogs()
        {
            var path = Path.Combine(Server.MapPath("~/Logs"), "LogFile068.txt");
            var sb = new StringBuilder();
            using (var stream = System.IO.File.Open(path,FileMode.Open,FileAccess.Read,FileShare.ReadWrite))
            {
                var logFileReader = new StreamReader(stream);

                while (!logFileReader.EndOfStream)
                {
                    sb.AppendLine(logFileReader.ReadLine());
                }

                // Clean up
                logFileReader.Close();
                stream.Close();
            }
            ViewBag.Log = sb.ToString();
            return View("ViewLogs");
        }
    }
}
using System;
using System.Web.Mvc;
using Couchbase;
using Couchbase.Linq;
using CouchbaseAspNetExample3.Models;

namespace CouchbaseAspNetExample3.Controllers
{
    public class HomeController : Controller
    {
        private readonly PersonRepository _personRepo;

        public HomeController()
        {
            var bucket = ClusterHelper.GetBucket("default");
            var bucketContext = new BucketContext(bucket);
            _personRepo = new PersonRepository(bucket, bucketContext);
        }

        public ActionResult Index()
        {
            var list = _personRepo.GetAll();
            return View(list);
        }

        public ActionResult Add()
        {
            return View("Edit", new Person());
        }

        [HttpPost]
        public ActionResult Save(Person model)
        {
            _personRepo.Save(model);
            return RedirectToAction("Index");
        }

        public ActionResult Edit(Guid id)
        {
            var person = _personRepo.GetPersonByKey(id);
            return View("Edit", person);
        }

        public ActionResult Delete(Guid id)
        {
            _personRepo.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
using System;
using System.Linq;
using System.Web.Mvc;
using CouchbaseServerDataAccess;
using SQLServerDataAccess;
using SQLServerToCouchbase.Core;
using SQLServerToCouchbase.Web.Models;

namespace SQLServerToCouchbase.Web.Controllers
{
    public class SocialMediaController : Controller
    {
        readonly ISocialMediaRepository _socialMediaRepo;

        public SocialMediaController()
        {
            if (MvcApplication.UseSQL)
                _socialMediaRepo = new SqlSocialMediaRepository();
            else
                _socialMediaRepo = new CouchbaseSocialMediaRepository();
        }

        public ViewResult Index()
        {
            var updates = _socialMediaRepo.GetTenLatestUpdates();
            return View(updates);
        }

        public RedirectToRouteResult SeedData()
        {
            _socialMediaRepo.SeedData();
            return RedirectToAction("Index");
        }

        public ViewResult UserPage(Guid id)
        {
            var userPageView = new UserPageView
            {
                Updates = _socialMediaRepo.GetTenLatestUpdatesForUser(id),
                User = _socialMediaRepo.GetUserById(id)
            };
            return View(userPageView);
        }

        [HttpPost]
        public RedirectToRouteResult UserPage(Guid userId, string body)
        {
            _socialMediaRepo.SendUpdate(userId, body);

            return RedirectToAction("UserPage", new {id = userId});
        }

        [HttpPost]
        public RedirectToRouteResult AddFriend(Guid userId, string friendName)
        {
            var friend = _socialMediaRepo.GetUserByName(friendName);
            if (friend == null)
            {
                TempData["Error"] = "User with name '" + friendName + "' not found.";
                return RedirectToAction("UserPage", new {id = userId});
            }

            if (friend.Id == userId)
            {
                TempData["Error"] = "You can't add yourself as a friend.";
                return RedirectToAction("UserPage", new { id = userId });
            }

            _socialMediaRepo.AddFriend(userId, friend.Id);

            return RedirectToAction("UserPage", new { id = userId });
        }
    }
}
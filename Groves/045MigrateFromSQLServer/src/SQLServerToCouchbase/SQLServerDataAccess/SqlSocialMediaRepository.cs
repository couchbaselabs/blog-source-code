using System;
using System.Collections.Generic;
using System.Linq;
using SQLServerToCouchbase.Core;
using SQLServerToCouchbase.Core.SocialMedia;

namespace SQLServerDataAccess
{
    public class SqlSocialMediaRepository : ISocialMediaRepository
    {
        private readonly SqlToCbContext _context;

        public SqlSocialMediaRepository()
        {
            _context = new SqlToCbContext();
        }

        public List<Update> GetTenLatestUpdates()
        {
            var updates = _context.FriendBookUpdates
                .OrderByDescending(u => u.PostedDate)
                .Take(10)
                .ToList();
            return updates;
        }

        public void SeedData()
        {
            // create user
            var user = new FriendbookUser
            {
                Id = Guid.NewGuid(),
                Friends = new List<FriendbookUser>(),
                Name = Faker.Name.FullName()
            };
            _context.FriendBookUsers.Add(user);
            _context.SaveChanges();

            // create update
            var update = new Update
            {
                Id = Guid.NewGuid(),
                Body = Faker.Lorem.Paragraph(1),
                User = user,
                PostedDate = DateTime.Now
            };
            _context.FriendBookUpdates.Add(update);
            _context.SaveChanges();
        }

        public List<Update> GetTenLatestUpdatesForUser(Guid id)
        {
            var updates = _context.FriendBookUpdates
                .Where(u => u.User.Id == id)
                .OrderByDescending(u => u.PostedDate)
                .Take(10)
                .ToList();
            return updates;
        }

        public void SendUpdate(Guid userId, string body)
        {
           // get user
            var user = _context.FriendBookUsers
                .SingleOrDefault(u => u.Id == userId);

            // create update
            var update = new Update
            {
                Id = Guid.NewGuid(),
                Body = body,
                User = user,
                PostedDate = DateTime.Now
            };
            _context.FriendBookUpdates.Add(update);
            _context.SaveChanges();
        }

        public FriendbookUser GetUserById(Guid id)
        {
            var user = _context.FriendBookUsers
                .Where(u => u.Id == id)
                .FirstOrDefault();
            return user;
        }

        public FriendbookUser GetUserByName(string friendName)
        {
            var user = _context.FriendBookUsers
                .Where(u => u.Name.ToLower() == friendName.ToLower())
                .FirstOrDefault();
            return user;
        }

        public void AddFriend(Guid userId, Guid friendId)
        {
            var user = GetUserById(userId);
            var friend = GetUserById(friendId);
            user.Friends.Add(friend);
            _context.SaveChanges();
        }
    }
}
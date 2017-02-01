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
            var updates = _context.FriendBookUpdates.OrderByDescending(u => u.PostedDate).Take(10).ToList();
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
            throw new NotImplementedException();
        }

        public void SendUpdate(Guid userId, string body)
        {
            throw new NotImplementedException();
        }

        public FriendbookUser GetUserById(Guid id)
        {
            throw new NotImplementedException();
        }

        public FriendbookUser GetUserByName(string friendName)
        {
            throw new NotImplementedException();
        }

        public void AddFriend(Guid userId, Guid friendId)
        {
            throw new NotImplementedException();
        }
    }
}
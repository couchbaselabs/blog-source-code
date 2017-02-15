using System;
using System.Linq;
using Couchbase;
using Couchbase.Core;
using SQLServerDataAccess;
using SQLServerToCouchbase.Core.SocialMedia;

namespace SQLServerToCouchbase.Migrate
{
    public class SocialMediaMigrator
    {
        private readonly IBucket _bucket;
        private readonly SqlToCbContext _context;

        public SocialMediaMigrator(IBucket bucket, SqlToCbContext context)
        {
            _bucket = bucket;
            _context = context;
        }

        public bool Go()
        {
            var users = _context.FriendBookUsers.ToList();
            foreach (var user in users)
            {
                var userDocument = new Document<dynamic>
                {
                    Id = user.Id.ToString(),
                    Content = MapUser(user)
                };
                var result = _bucket.Insert(userDocument);
                if (!result.Success)
                {
                    Console.WriteLine($"There was an error migrating User {user.Id}");
                    return false;
                }
                else
                    Console.WriteLine($"Successfully migrated User {user.Id}");
            }

            var updates = _context.FriendBookUpdates.ToList();
            foreach (var update in updates)
            {
                var updateDocument = new Document<dynamic>
                {
                    Id = update.Id.ToString(),
                    Content = MapUpdate(update)
                };
                var result = _bucket.Insert(updateDocument);
                if (!result.Success)
                {
                    Console.WriteLine($"There was an error migrating Update {update.Id}");
                    return false;
                }
                Console.WriteLine($"Successfully migrated Update {update.Id}");
            }

            return true;
        }

        private dynamic MapUpdate(Update update)
        {
            return new
            {
                Body = update.Body,
                PostedDate = update.PostedDate,
                UserId = update.UserId,
                Type = "Update"
            };
        }

        private dynamic MapUser(FriendbookUser user)
        {
            return new
            {
                Friends = user.Friends.Select(f => f.Id),
                Name = user.Name,
                Type = "User"
            };
        }

        public void Cleanup()
        {
            Console.WriteLine("Delete all users...");
            var result = _bucket.Query<dynamic>("DELETE FROM `sqltocb` WHERE type='User';");
            if (!result.Success)
            {
                Console.WriteLine($"{result.Exception?.Message}");
                Console.WriteLine($"{result.Message}");
            }
            Console.WriteLine("Delete all updates...");
            result = _bucket.Query<dynamic>("DELETE FROM `sqltocb` WHERE type='Update';");
            if (!result.Success)
            {
                Console.WriteLine($"{result.Exception?.Message}");
                Console.WriteLine($"{result.Message}");
            }
        }
    }
}
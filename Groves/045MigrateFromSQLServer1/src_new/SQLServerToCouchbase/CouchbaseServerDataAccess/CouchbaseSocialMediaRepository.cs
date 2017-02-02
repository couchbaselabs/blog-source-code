using System;
using System.Collections.Generic;
using System.Linq;
using Couchbase;
using Couchbase.Core;
using Couchbase.N1QL;
using SQLServerToCouchbase.Core;
using SQLServerToCouchbase.Core.SocialMedia;

namespace CouchbaseServerDataAccess
{
    public class CouchbaseSocialMediaRepository : ISocialMediaRepository
    {
        private readonly IBucket _bucket;

        public CouchbaseSocialMediaRepository()
        {
            _bucket = ClusterHelper.GetBucket("sqltocb");
        }

        public List<Update> GetTenLatestUpdates()
        {
            var n1ql = @"SELECT up.body, up.postedDate, TOOBJECT({ 'id': META(u).id, u.name}) AS `user`
                FROM `sqltocb` up
                JOIN `sqltocb` u ON KEYS up.`user`
                WHERE up.type = 'Update'
                ORDER BY STR_TO_MILLIS(up.postedDate) DESC
                LIMIT 10;";
            var query = QueryRequest.Create(n1ql);
            query.ScanConsistency(ScanConsistency.RequestPlus);
            var result = _bucket.Query<Update>(query);
            return result.Rows;
        }

        public void SeedData()
        {
            // create user
            var userId = Guid.NewGuid().ToString();
            var user = new FriendbookUser
            {
                Friends = new List<FriendbookUser>(),
                Name = Faker.Name.FullName()
            };
            _bucket.Insert(new Document<dynamic>
            {
                Id = userId,
                Content = new
                {
                    Friends = new List<dynamic>(),
                    Name = user.Name,
                    Type = "User"
                }
            });

            // create update
            var update = new Update
            {
                Body = Faker.Lorem.Paragraph(1),
                User = user,
                PostedDate = DateTime.Now
            };
            _bucket.Insert(new Document<dynamic>
            {
                Id = Guid.NewGuid().ToString(),
                Content = new
                {
                    Body = update.Body,
                    PostedDate = update.PostedDate,
                    User = userId,
                    Type = "Update"
                }
            });
        }

        public List<Update> GetTenLatestUpdatesForUser(Guid id)
        {
            var n1ql = @"SELECT up.body, up.postedDate, TOOBJECT({ 'id': META(u).id, u.name}) AS `user`
                FROM `sqltocb` up
                JOIN `sqltocb` u ON KEYS up.`user`
                WHERE up.type = 'Update'
                AND META(u).id = $userId
                ORDER BY STR_TO_MILLIS(up.postedDate) DESC
                LIMIT 10;";
            var query = QueryRequest.Create(n1ql);
            query.ScanConsistency(ScanConsistency.RequestPlus);
            query.AddNamedParameter("userId", id);
            var result = _bucket.Query<Update>(query);
            return result.Rows;
        }

        public void SendUpdate(Guid userId, string body)
        {
            _bucket.Insert(new Document<dynamic>
            {
                Id = Guid.NewGuid().ToString(),
                Content = new
                {
                    Body = body,
                    PostedDate = DateTime.Now,
                    User = userId.ToString(),
                    Type = "Update"
                }
            });
        }

        public FriendbookUser GetUserById(Guid id)
        {
            var n1ql = @"SELECT META(u).id AS id, u.name, ARRAY { friend.name, META(friend).id } FOR friend IN f END AS `friends`
                FROM `sqltocb` u
                LEFT NEST `sqltocb` f ON KEYS u.friends
                WHERE META(u).id = $userId
                LIMIT 1;";
            var query = QueryRequest.Create(n1ql);
            query.ScanConsistency(ScanConsistency.RequestPlus);
            query.AddNamedParameter("userId", id);
            var result = _bucket.Query<FriendbookUser>(query);
            return result.Rows.FirstOrDefault();
        }

        public FriendbookUser GetUserByName(string friendName)
        {
            var n1ql = @"SELECT u.name, META(u).id AS id
                FROM `sqltocb` u
                WHERE LOWER(u.name) = $friendName
                LIMIT 1;";
            var query = QueryRequest.Create(n1ql);
            query.ScanConsistency(ScanConsistency.RequestPlus);
            query.AddNamedParameter("friendName", friendName.ToLower());
            var result = _bucket.Query<FriendbookUser>(query);
            return result.Rows.FirstOrDefault();
        }

        public void AddFriend(Guid userId, Guid friendId)
        {
            _bucket.MutateIn<dynamic>(userId.ToString())
                .ArrayAddUnique("friends", friendId.ToString())
                .Execute();
        }
    }
}
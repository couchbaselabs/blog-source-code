using System;
using System.Collections.Generic;
using SQLServerToCouchbase.Core;
using SQLServerToCouchbase.Core.SocialMedia;

namespace SQLServerDataAccess
{
    public class SqlSocialMediaRepository : ISocialMediaRepository
    {
        public List<Update> GetTenLatestUpdates()
        {
            throw new System.NotImplementedException();
        }

        public void SeedData()
        {
            throw new System.NotImplementedException();
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
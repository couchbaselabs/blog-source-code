using System;
using System.Collections.Generic;
using SQLServerToCouchbase.Core.SocialMedia;

namespace SQLServerToCouchbase.Core
{
    public interface ISocialMediaRepository
    {
        List<Update> GetTenLatestUpdates();
        void SeedData();
        List<Update> GetTenLatestUpdatesForUser(Guid id);
        void SendUpdate(Guid userId, string body);
        FriendbookUser GetUserById(Guid id);
        FriendbookUser GetUserByName(string friendName);
        void AddFriend(Guid userId, Guid friendId);
    }
}
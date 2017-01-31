using System;
using System.Collections.Generic;

namespace SQLServerToCouchbase.Core.SocialMedia
{
    public class FriendbookUser
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<FriendbookUser> Friends { get; set; }
    }
}
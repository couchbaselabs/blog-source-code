using System;
using System.Collections.Generic;

namespace SQLServerToCouchbase.Core.SocialMedia
{
    // tag::FriendbookUser[]
    public class FriendbookUser
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public virtual List<FriendbookUser> Friends { get; set; }
    }
    // end::FriendbookUser[]
}
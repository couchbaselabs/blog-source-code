using System;

namespace SQLServerToCouchbase.Core.SocialMedia
{
    public class Update
    {
        public int Id { get; set; }
        public DateTime PostedDate { get; set; }
        public string Body { get; set; }
        public FriendbookUser User { get; set; }
    }
}
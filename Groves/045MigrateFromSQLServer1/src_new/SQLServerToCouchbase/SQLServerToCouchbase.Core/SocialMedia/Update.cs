using System;
using Newtonsoft.Json;

namespace SQLServerToCouchbase.Core.SocialMedia
{
    public class Update
    {
        public Guid Id { get; set; }
        public DateTime PostedDate { get; set; }
        public string Body { get; set; }
        public virtual FriendbookUser User { get; set; }
        public Guid UserId { get; set; }
    }
}
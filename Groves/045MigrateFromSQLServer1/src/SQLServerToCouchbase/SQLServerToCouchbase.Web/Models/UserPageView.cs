using System.Collections.Generic;
using SQLServerToCouchbase.Core.SocialMedia;

namespace SQLServerToCouchbase.Web.Models
{
    public class UserPageView
    {
        public List<Update> Updates { get; set; }
        public FriendbookUser User { get; set; }
    }
}
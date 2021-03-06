﻿using System;

namespace SQLServerToCouchbase.Core.SocialMedia
{
    // tag::Update[]
    public class Update
    {
        public Guid Id { get; set; }
        public DateTime PostedDate { get; set; }
        public string Body { get; set; }

        public virtual FriendbookUser User { get; set; }
        public Guid UserId { get; set; }
    }
    // end::Update[]
}
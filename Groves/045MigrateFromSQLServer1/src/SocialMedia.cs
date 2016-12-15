using System;
using System.Collections.Generic;

namespace ConsoleApplication
{
    // tag::SocialMediaModel[]
    public class FriendbookUser
    {
        public int Id { get; set; }
        public string Name { get; set;}
        public List<FriendbookUser> Friends { get; set; }
        public List<Update> Updates { get; set; }
    }

    public class Update
    {
        public int Id { get; set; }
        public DateTime PostedDate { get; set; }
        public string Body { get; set; }
    }
    // end::SocialMediaModel[]

    // tag::Mappings[]
    public class FriendbookUserMap : ClassMap<FriendbookUser>
    {
        public FriendbookUserMap()
        {
            UseTable("FriendbookUser");
            
            Id(x => x.Id);
            Map(x => x.Name);
            HasMany(x => x.Friends);
            HasMany(x => x.Updates);
        }
    }

    public class UpdateMap : ClassMap<Update> {
        public UpdateMap()
        {
            UseTable("Update");

            Id(x => x.Id);
            Map(x => x.PostedDate);
            Map(x => x.Body);
        }
    }
    // end::Mappings[]
}
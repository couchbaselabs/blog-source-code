using System.Data.Entity.ModelConfiguration;
using SQLServerToCouchbase.Core.SocialMedia;

namespace SQLServerDataAccess
{
    // tag::Map[]
    public class FriendbookUserMap : EntityTypeConfiguration<FriendbookUser>
    {
        public FriendbookUserMap()
        {
            this.HasKey(m => m.Id);

            this.ToTable("FriendBookUsers");
            this.Property(m => m.Name);
            this.HasMany(t => t.Friends)
                .WithMany()
                .Map(m =>
                {
                    m.MapLeftKey("UserId");
                    m.MapRightKey("FriendUserId");
                    m.ToTable("FriendBookUsersFriends");
                });
        }
    }
    // end::Map[]
}
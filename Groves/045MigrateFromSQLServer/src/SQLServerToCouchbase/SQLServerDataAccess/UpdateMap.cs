using System.Data.Entity.ModelConfiguration;
using SQLServerToCouchbase.Core.SocialMedia;

namespace SQLServerDataAccess
{
    // tag::Map[]
    public class UpdateMap : EntityTypeConfiguration<Update>
    {
        public UpdateMap()
        {
            this.HasKey(m => m.Id);

            this.ToTable("FriendBookUpdates");
            this.Property(m => m.Body);
            this.Property(m => m.PostedDate);
            this.HasRequired(m => m.User)
                .WithMany()
                .HasForeignKey(m => m.UserId);
        }
    }
    // end::Map[]
}
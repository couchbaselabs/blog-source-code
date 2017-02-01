using System;
using System.Data.Entity.ModelConfiguration;
using SQLServerToCouchbase.Core.Shopping;
using SQLServerToCouchbase.Core.SocialMedia;

namespace SQLServerDataAccess
{
    using System.Data.Entity;

    public class SqlToCbContext : DbContext
    {
        public SqlToCbContext() : base("name=CodeFirstModel")
        {
        }

        public virtual DbSet<Update> FriendBookUpdates { get; set; }
        public virtual DbSet<FriendbookUser> FriendBookUsers { get; set; }
        public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public virtual DbSet<Item> ShoppingCartItems { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new FriendbookUserMap());
            modelBuilder.Configurations.Add(new UpdateMap());
            modelBuilder.Configurations.Add(new ShoppingCartMap());
            modelBuilder.Configurations.Add(new ShoppingCartItemMap());
        }
    }

    public class ShoppingCartItemMap : EntityTypeConfiguration<Item>
    {
        public ShoppingCartItemMap()
        {
            this.HasKey(m => m.Id);

            this.ToTable("ShoppingCartItems");
            this.Property(m => m.Name);
            this.Property(m => m.Price);
            this.Property(m => m.Quantity);
        }
    }

    public class ShoppingCartMap : EntityTypeConfiguration<ShoppingCart>
    {

        public ShoppingCartMap()
        {
            this.HasKey(m => m.Id);

            this.ToTable("ShoppingCart");
            this.Property(m => m.User);
            this.Property(m => m.DateCreated);
            this.HasMany(m => m.Items)
                .WithOptional()
                .HasForeignKey(m => m.ShoppingCartId);
        }
    }

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
}

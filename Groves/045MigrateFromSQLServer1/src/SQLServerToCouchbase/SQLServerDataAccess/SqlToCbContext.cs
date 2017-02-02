using System;
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
}

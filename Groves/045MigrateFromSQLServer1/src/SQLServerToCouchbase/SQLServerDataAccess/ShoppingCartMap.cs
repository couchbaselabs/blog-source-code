using System.Data.Entity.ModelConfiguration;
using SQLServerToCouchbase.Core.Shopping;

namespace SQLServerDataAccess
{
    // tag::Map[]
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
            // end::Map[]

            this.Ignore(m => m.Type);     // Type field is only for Couchbase
            // tag::Close[]
        }
    }
    // end::Close[]
}
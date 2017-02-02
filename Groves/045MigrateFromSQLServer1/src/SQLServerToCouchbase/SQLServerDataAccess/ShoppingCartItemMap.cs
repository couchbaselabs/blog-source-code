using System.Data.Entity.ModelConfiguration;
using SQLServerToCouchbase.Core.Shopping;

namespace SQLServerDataAccess
{
    // tag::Map[]
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
    // end::Map[]
}
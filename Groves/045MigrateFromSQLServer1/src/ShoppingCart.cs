using System;
using System.Collections.Generic;
using Couchbase.Core;

namespace ConsoleApplication
{
    // tag::Classes[]
    public class ShoppingCart
    {
        public int Id { get; set; }
        public string User { get; set;}
        public DateTime DateCreated { get; set;}

        public List<Item> Items { get; set; }
    }

    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
    // end::Classes[]

    // tag::Mappings[]
    public class ShoppingCartMap : ClassMap<ShoppingCart>
    {
        public ShoppingCartMap()
        {
            UseTable("ShoppingCart");

            Id(x => x.Id);
            Map(x => x.User);
            Map(x => x.DateCreated);
            HasMany(x => x.Items);
        }
    }

    public class ShoppingCartItemMap : ClassMap<Item>
    {
        public ShoppingCartItemMap()
        {
            UseTable("ShoppingCartItem");
            
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.Price);
        }
    }
    // end::Mappings[]

    public class ShoppingCartRepository
    {
        IBucket _bucket;
        public ShoppingCartRepository(IBucket bucket)
        {
            _bucket = bucket;
        }

        // tag::GetShoppingCart[]
        public ShoppingCart GetShoppingCart(string key)
        {
            return _bucket.Get<ShoppingCart>(key).Value;
        }
        // end::GetShoppingCart[]
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Couchbase;
using Couchbase.Core;
using SQLServerDataAccess;
using SQLServerToCouchbase.Core.Shopping;
using System.Data.Entity;

namespace SQLServerToCouchbase.Migrate
{
    // tag::ShoppingCartMigratorTop[]
    public class ShoppingCartMigrator
    {
        readonly IBucket _bucket;
        readonly SqlToCbContext _context;

        public ShoppingCartMigrator(IBucket bucket, SqlToCbContext context)
        {
            _bucket = bucket;
            _context = context;
        }
        // end::ShoppingCartMigratorTop[]

        // tag::ShoppingCartMigratorGo[]
        public bool Go()
        {
            var carts = _context.ShoppingCarts
                .Include(x => x.Items)
                .ToList();
            foreach (var cart in carts)
            {
                var cartDocument = new Document<dynamic>
                {
                    Id = cart.Id.ToString(),
                    Content = MapCart(cart)
                };
                var result = _bucket.Insert(cartDocument);
                if (!result.Success)
                {
                    Console.WriteLine($"There was an error migrating Shopping Cart {cart.Id}");
                    return false;
                }
                Console.WriteLine($"Successfully migrated Shopping Cart {cart.Id}");
            }
            return true;
        }
        // end::ShoppingCartMigratorGo[]

        private dynamic MapCart(ShoppingCart cart)
        {
            return new
            {
                DateCreated = cart.DateCreated,
                Type = "ShoppingCart",
                User = cart.User,
                Items = MapItems(cart.Items)
            };
        }

        private IEnumerable<dynamic> MapItems(List<Item> items)
        {
            foreach (var item in items)
            {
                yield return new
                {
                    Name = item.Name,
                    Price = item.Price,
                    Quantity = item.Quantity
                };
            }
        }

        // tag::ShoppingCartMigratorRollback[]
        public void Rollback()
        {
            Console.WriteLine("Delete all shopping carts...");
            var result = _bucket.Query<dynamic>("DELETE FROM `sqltocb` WHERE type='ShoppingCart';");
            if (!result.Success)
            {
                Console.WriteLine($"{result.Exception?.Message}");
                Console.WriteLine($"{result.Message}");
            }
        }
        // end::ShoppingCartMigratorRollback[]
        // tag::ShoppingCartMigratorBottom[]
    }
    // end::ShoppingCartMigratorBottom[]
}
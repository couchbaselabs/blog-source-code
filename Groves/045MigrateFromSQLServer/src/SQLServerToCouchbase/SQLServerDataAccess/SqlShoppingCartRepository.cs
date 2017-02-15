using System;
using System.Collections.Generic;
using System.Linq;
using SQLServerToCouchbase.Core;
using SQLServerToCouchbase.Core.Shopping;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

namespace SQLServerDataAccess
{
    public class SqlShoppingCartRepository : IShoppingCartRepository
    {
        private readonly SqlToCbContext _context;

        public SqlShoppingCartRepository()
        {
            _context = new SqlToCbContext();
        }

        public List<ShoppingCart> GetTenLatestShoppingCarts()
        {
            var carts = _context.ShoppingCarts
                .Include(x => x.Items)      // eager load items
                .OrderByDescending(c => c.DateCreated)
                .Take(10)
                .ToList();
            return carts;
        }

        public void SeedEmptyShoppingCart()
        {
            var cart = new ShoppingCart
            {
                Id = Guid.NewGuid(),
                User = Faker.Name.First().ToLower()[0] + Faker.Name.Last().ToLower(),
                // format first initial + last name, e.g. "mgroves"
                DateCreated = DateTime.Now,
            };
            _context.ShoppingCarts.Add(cart);
            _context.SaveChanges();
        }

        public ShoppingCart GetCartById(Guid id)
        {
            var cart = _context.ShoppingCarts
                .Include(x => x.Items)      // eager load items
                .Where(c => c.Id == id)
                .SingleOrDefault();
            return cart;
        }

        public void AddItemToCart(Guid cartId, Item item)
        {
            var cart = GetCartById(cartId);
            cart.Items = cart.Items ?? new List<Item>();
            cart.Items.Add(item);
            _context.SaveChanges();
        }

        // tag::Sproc[]
        public List<ShoppingCart> SearchForCartsByUserName(string searchString)
        {
            var cmd = _context.Database.Connection.CreateCommand();
            cmd.CommandText = "SP_SEARCH_SHOPPING_CART_BY_NAME @searchString";
            cmd.Parameters.Add(new SqlParameter("@searchString", searchString));
            _context.Database.Connection.Open();
            var reader = cmd.ExecuteReader();

            var carts = ((IObjectContextAdapter) _context)
                .ObjectContext
                .Translate<ShoppingCart>(reader, "ShoppingCarts", MergeOption.AppendOnly);

            var result = carts.ToList();
            _context.Database.Connection.Close();
            return result;
        }
        // end::Sproc[]
    }
}
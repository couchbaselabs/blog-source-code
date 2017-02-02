using System;
using System.Collections.Generic;
using System.Linq;
using SQLServerToCouchbase.Core;
using SQLServerToCouchbase.Core.Shopping;
using System.Data.Entity;

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
    }
}
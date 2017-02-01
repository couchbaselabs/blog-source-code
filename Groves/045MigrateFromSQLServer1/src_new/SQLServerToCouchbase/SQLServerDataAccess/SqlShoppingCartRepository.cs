using System;
using System.Collections.Generic;
using SQLServerToCouchbase.Core;
using SQLServerToCouchbase.Core.Shopping;

namespace SQLServerDataAccess
{
    public class SqlShoppingCartRepository : IShoppingCartRepository
    {
        public List<ShoppingCart> GetTenLatestShoppingCarts()
        {
            throw new System.NotImplementedException();
        }

        public void SeedEmptyShoppingCart()
        {
            throw new System.NotImplementedException();
        }

        public ShoppingCart GetCartById(Guid id)
        {
            throw new NotImplementedException();
        }

        public void AddItemToCart(Guid cartId, Item item)
        {
            throw new NotImplementedException();
        }
    }
}
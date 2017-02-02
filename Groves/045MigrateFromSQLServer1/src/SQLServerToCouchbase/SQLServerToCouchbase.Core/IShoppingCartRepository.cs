using System;
using System.Collections.Generic;
using SQLServerToCouchbase.Core.Shopping;

namespace SQLServerToCouchbase.Core
{
    public interface IShoppingCartRepository
    {
        List<ShoppingCart> GetTenLatestShoppingCarts();
        void SeedEmptyShoppingCart();
        ShoppingCart GetCartById(Guid id);
        void AddItemToCart(Guid cartId, Item item);
    }
}
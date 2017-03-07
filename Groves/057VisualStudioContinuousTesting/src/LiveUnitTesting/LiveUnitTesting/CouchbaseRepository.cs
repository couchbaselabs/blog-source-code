using Couchbase;
using Couchbase.Core;
using System;

namespace ContinuousTesting
{
    public class CouchbaseRepository
    {
        private readonly IBucket _bucket;

        public CouchbaseRepository(IBucket bucket)
        {
            _bucket = bucket;
        }

        public void SaveShoppingCart(ShoppingCart cart)
        {
            _bucket.Insert(new Document<ShoppingCart>
            {
                Id = Guid.NewGuid().ToString(),
                Content = cart
            });
        }

        public void AddItem(Guid shoppingCartId, Item item)
        {
            var cart = _bucket.Get<ShoppingCart>(shoppingCartId.ToString()).Value;
            cart.Items.Add(item);
            _bucket.Replace(new Document<ShoppingCart>
            {
                Id = shoppingCartId.ToString(),
                Content = cart
            });
        }
    }
}

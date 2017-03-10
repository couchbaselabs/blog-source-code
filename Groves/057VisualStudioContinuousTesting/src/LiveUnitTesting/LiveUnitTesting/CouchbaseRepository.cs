using Couchbase;
using Couchbase.Core;
using Couchbase.N1QL;
using System;
using System.Linq;

namespace ContinuousTesting
{
    public class CouchbaseRepository
    {
        private readonly IBucket _bucket;

        public CouchbaseRepository(IBucket bucket)
        {
            _bucket = bucket;
        }

        // tag::SaveShoppingCart[]
        public void SaveShoppingCart(ShoppingCart cart)
        {
            _bucket.Insert(new Document<ShoppingCart>
            {
                Id = Guid.NewGuid().ToString(),
                Content = cart
            });
        }
        // end::SaveShoppingCart[]

        public ShoppingCart GetCartByUserName(string userName)
        {
            var n1ql = $"SELECT c.* FROM `{_bucket.Name}` c WHERE c.userName = $UserName LIMIT 1";
            var query = QueryRequest.Create(n1ql);
            query.ScanConsistency(ScanConsistency.RequestPlus);
            query.AddNamedParameter("UserName", userName);
            return _bucket.Query<ShoppingCart>(query).Rows.FirstOrDefault();
        }
    }
}

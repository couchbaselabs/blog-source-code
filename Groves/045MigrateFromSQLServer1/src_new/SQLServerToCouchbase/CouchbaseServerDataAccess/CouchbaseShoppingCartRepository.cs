using System;
using System.Collections.Generic;
using System.Linq;
using Couchbase;
using Couchbase.Core;
using Couchbase.Linq;
using Couchbase.Linq.Extensions;
using Couchbase.N1QL;
using SQLServerToCouchbase.Core;
using SQLServerToCouchbase.Core.Shopping;

namespace CouchbaseServerDataAccess
{
    public class CouchbaseShoppingCartRepository : IShoppingCartRepository
    {
        private readonly IBucket _bucket;
        private readonly BucketContext _context;

        public CouchbaseShoppingCartRepository()
        {
            _bucket = ClusterHelper.GetBucket("sqltocb");
            _context = new BucketContext(_bucket);
        }

        public List<ShoppingCart> GetTenLatestShoppingCarts()
        {
            // this code uses the standard Couchbase .NET SDK
            /*
            var n1ql = @"SELECT META(c).id, c.*
                FROM `sqltocb` c
                WHERE c.type = 'ShoppingCart'
                ORDER BY STR_TO_MILLIS(c.dateCreated) DESC
                LIMIT 10;";
            var query = QueryRequest.Create(n1ql);
            query.ScanConsistency(ScanConsistency.RequestPlus);
            return _bucket.Query<ShoppingCart>(query).Rows;
            */

            // this code uses Linq2Couchbase
            var query = from c in _context.Query<ShoppingCart>()
                where c.Type == "ShoppingCart"  // could use DocumentFilter attribute instead of this Where
                orderby c.DateCreated descending
                select new {Cart = c, Id = N1QlFunctions.Meta(c).Id};
            var results = query.ScanConsistency(ScanConsistency.RequestPlus)
                .Take(10)
                .ToList();
            results.ForEach(r => r.Cart.Id = Guid.Parse(r.Id));
            return results.Select(r => r.Cart).ToList();
        }

        public void SeedEmptyShoppingCart()
        {
            _bucket.Insert(new Document<dynamic>
            {
                Id = Guid.NewGuid().ToString(),
                Content = new
                {
                    User = Faker.Name.First().ToLower()[0] + Faker.Name.Last().ToLower(), // format first initial + last name, e.g. "mgroves"
                    DateCreated = DateTime.Now,
                    Items = new List<Item>(),
                    Type = "ShoppingCart"
                }
            });
        }

        public ShoppingCart GetCartById(Guid id)
        {
            return _bucket.Get<ShoppingCart>(id.ToString()).Value;
        }

        public void AddItemToCart(Guid cartId, Item item)
        {
            // note that since I'm using the Item class
            // which is also being used for SQL in this demo
            // that there will be an "Id" field serialized to Couchbase
            // However, this Id field is completely unnecessary for Couchbase
            // and will always be '0' when in couchbase
            _bucket.MutateIn<ShoppingCart>(cartId.ToString())
                .ArrayAppend("items", item)
                .Execute();
        }
    }
}
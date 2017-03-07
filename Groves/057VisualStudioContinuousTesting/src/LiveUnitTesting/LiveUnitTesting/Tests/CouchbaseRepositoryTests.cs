using Couchbase;
using Couchbase.Configuration.Client;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace ContinuousTesting.Tests
{
    [TestFixture]
    public class CouchbaseRepositoryTests
    {
        CouchbaseRepository Repo;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            ClusterHelper.Initialize(new ClientConfiguration
            {
                Servers = new List<Uri> { new Uri("couchbase://localhost") }
            });
            var bucket = ClusterHelper.GetBucket("default");
            Repo = new CouchbaseRepository(bucket);
        }

        [Test]
        public void SmokeTest()
        {
            Repo.SaveShoppingCart(new ShoppingCart
            {
                UserName = "Matthew " + Guid.NewGuid().ToString(),
                LastUpdated = DateTime.Now
            });
        }
    }
}

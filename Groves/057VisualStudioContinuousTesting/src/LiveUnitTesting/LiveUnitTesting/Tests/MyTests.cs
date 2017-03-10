using Couchbase;
using Couchbase.Configuration.Client;
using LiveUnitTesting.Tests;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace ContinuousTesting.Tests
{
    [TestFixture]
    public class MyTests
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

        // tag::IgnoreAttributeUsage[]
        [IgnoreForLiveTesting("Integration Test")]
        // end::IgnoreAttributeUsage[]
        // tag::IntegrationTest[]
        [Test]
        public void Repo_Can_Save_a_New_Shopping_Cart_to_Database()
        {
            // arrange: create a shopping cart
            var cart = new ShoppingCart
            {
                UserName = "Matthew " + Guid.NewGuid().ToString(),
                LastUpdated = DateTime.Now
            };

            // act: save shopping cart to database
            Repo.SaveShoppingCart(cart);

            // assert: check that the cart was saved
            var cartBackOut = Repo.GetCartByUserName(cart.UserName);
            Assert.That(cartBackOut, Is.Not.Null);
            Assert.That(cartBackOut.UserName, Is.EqualTo(cart.UserName));
        }
        // end::IntegrationTest[]

        // tag::ShoppingCart_Total_Should_Sum_Up_the_Item_Prices[]
        [Test]
        public void ShoppingCart_Total_Should_Sum_Up_the_Item_Prices()
        {
            // arrange: create shopping cart with 2 items and figure out the expected total
            var item1 = new Item { Name = "Large Pepperoni Pizza", Price = 14.99M };
            var item2 = new Item { Name = "Cheese Sticks", Price = 4.99M };
            var expectedTotal = item1.Price + item2.Price;
            var cart = new ShoppingCart { Items = new List<Item> { item1, item2 } };

            // act: user the Total method on ShoppingCart
            var actualTotal = cart.Total;

            // assert: totals should match
            Assert.That(actualTotal, Is.EqualTo(expectedTotal));
        }
        // end::ShoppingCart_Total_Should_Sum_Up_the_Item_Prices[]
    }
}

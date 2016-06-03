using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Couchbase;
using Couchbase.Configuration.Client;
using Couchbase.Core;
using Couchbase.N1QL;

namespace ScanConsistencyDemo
{
    class Program
    {
        private static IBucket _bucket;
        private static Random _random = new Random();

        static void Main(string[] args)
        {
            SetupCouchbase();

//            NotBoundedExample();
//
//            RequestPlusExample();

            AtPlusExample();

            CleanupCouchbase();
        }

        // tag::NotBoundedExample[]
        private static void NotBoundedExample()
        {
            Console.WriteLine("========= NonBounded (default)");
            // get the current count
            var result1 =
                _bucket.Query<dynamic>("SELECT COUNT(1) as airportCount FROM `travel-sample` WHERE type='airport'")
                    .Rows.First();
            Console.WriteLine($"Initial count: {result1.airportCount}");

            // insert a new airport
            var doc = new Document<dynamic>
            {
                Id = "ScanConsistency::airport::" + _random.Next(10000),
                Content = new
                {
                    type = "airport"
                }
            };
            _bucket.Insert(doc);

            // get the count again
            var result2 =
                _bucket.Query<dynamic>("SELECT COUNT(1) as airportCount FROM `travel-sample` WHERE type='airport'")
                    .Rows.First();
            Console.WriteLine($"Count after insert: {result2.airportCount}");

            // wait a few seconds and get the count again
            Console.Write("Waiting for 5 seconds...");
            Thread.Sleep(5000);
            var result3 =
                _bucket.Query<dynamic>("SELECT COUNT(1) as airportCount FROM `travel-sample` WHERE type='airport'")
                    .Rows.First();
            Console.WriteLine($"Count after waiting: {result3.airportCount}");
        }
        // end::NotBoundedExample[]

        // tag::RequestPlusExample[]
        private static void RequestPlusExample()
        {
            Console.WriteLine("========= RequestPlus");

            // get the current count
            var result1 =
                _bucket.Query<dynamic>("SELECT COUNT(1) as airportCount FROM `travel-sample` WHERE type='airport'")
                    .Rows.First();
            Console.WriteLine($"Initial count: {result1.airportCount}");

            // insert a new airport
            var doc = new Document<dynamic>
            {
                Id = "ScanConsistency::airport::" + _random.Next(10000),
                Content = new
                {
                    type = "airport"
                }
            };
            _bucket.Insert(doc);

            // get the count again
            var request =
                QueryRequest.Create("SELECT COUNT(1) as airportCount FROM `travel-sample` WHERE type='airport'");
            request.ScanConsistency(ScanConsistency.RequestPlus);
            var result2 = _bucket.Query<dynamic>(request).Rows.First();
            Console.WriteLine($"Count after insert with RequestPlus: {result2.airportCount}");
        }
        // end::RequestPlusExample[]

        // tag::AtPlusExample[]
        private static void AtPlusExample()
        {
            Console.WriteLine("========= AtPlus");

            // get the current count
            var result1 =
                _bucket.Query<dynamic>("SELECT COUNT(1) as airportCount FROM `travel-sample` WHERE type='airport'")
                    .Rows.First();
            Console.WriteLine($"Initial count: {result1.airportCount}");

            // insert a new airport
            var doc = new Document<dynamic>
            {
                Id = "ScanConsistency::airport::" + _random.Next(10000),
                Content = new
                {
                    type = "airport"
                }
            };
            var insertResult = _bucket.Insert(doc);

            // get the count again
            var state = MutationState.From(insertResult.Document);
            var request = new QueryRequest("SELECT COUNT(1) as airportCount FROM `travel-sample` WHERE type='airport'")
                .ConsistentWith(state);
            var result2 = _bucket.Query<dynamic>(request).Rows.First();
            Console.WriteLine($"Count after insert with AtPlus: {result2.airportCount}");
        }
        // end::AtPlusExample[]

        private static void CleanupCouchbase()
        {
            ClusterHelper.Close();
        }

        private static void SetupCouchbase()
        {

            var config = new ClientConfiguration();
            config.Servers = new List<Uri>
            {
                new Uri("http://localhost:8091")
            };
            config.UseSsl = false;
            ClusterHelper.Initialize(config);
            _bucket = ClusterHelper.GetBucket("travel-sample");
            _bucket.Configuration.UseEnhancedDurability = true;
        }
    }
}

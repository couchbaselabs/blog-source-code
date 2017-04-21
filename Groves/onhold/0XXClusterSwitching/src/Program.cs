using System;
using System.Collections.Generic;
using Couchbase;
using Couchbase.Authentication;
using Couchbase.Configuration.Client;
using Couchbase.Core;
using Couchbase.Management;

namespace multicluster
{
    class Program
    {
        static void Main(string[] args)
        {
            var bucketHelper = new BucketHelper();

            while (true)
            {
                Console.WriteLine("Type in some text. Enter QUIT to end.");
                var text = Console.ReadLine();
                if (text == "QUIT")
                    return;

                var bucket = bucketHelper.GetBucket("default");
                bucket.Insert(new Document<dynamic>
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = new
                    {
                        Text = text,
                        TimeStamp = DateTime.Now
                    }
                });
            }
        }
    }

    public class BucketHelper
    {
        private readonly ClientConfiguration _cluster1config;
        private readonly ClientConfiguration _cluster2config;

        private Cluster _cluster1;
        private Cluster _cluster2;
        private IClusterManager _cluster1Man;

        public BucketHelper()
        {
            _cluster1config = new ClientConfiguration
            {
                Servers = new List<Uri> {  new Uri("http://localhost:8091")}
            };

            _cluster2config = new ClientConfiguration
            {
                Servers = new List<Uri> { new Uri("http://192.168.1.64:8091") }
            };

            TryCreateCluster1();
            TryCreateCluster2();
        }

        private void TryCreateCluster1()
        {
            try
            {
                _cluster1 = new Cluster(_cluster1config);
                _cluster1.Authenticate(new ClassicAuthenticator("Administrator","password"));
                _cluster1Man = _cluster1.CreateManager();
            }
            catch (Exception)
            {
                // notifications, logging, etc
                Console.WriteLine("Error creating cluster 1");
            }
        }

        private void TryCreateCluster2()
        {
            try
            {
                _cluster2 = new Cluster(_cluster2config);
                _cluster2.Authenticate(new ClassicAuthenticator("Administrator", "password"));
            }
            catch (Exception)
            {
                // notifications, logging, etc
                Console.WriteLine("Error creating cluster 2");
            }
        }

        public IBucket GetBucket(string bucketName)
        {
            var cluster1info = _cluster1Man.ClusterInfo().Value;

            // if cluster 1 hasn't been working, try to spin it up again
            // may want to only check this every N minutes or something
            if (_cluster1 == null || cluster1info == null)
                TryCreateCluster1();

            // cluster 1 still not working, so fall back to cluster 2
            if (_cluster1 == null || cluster1info == null)
                return _cluster2.OpenBucket(bucketName, "");

            return _cluster1.OpenBucket(bucketName, "");
        }
    }
}
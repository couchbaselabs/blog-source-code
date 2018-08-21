using System;
using System.Collections.Generic;
using Couchbase;
using Couchbase.Configuration.Client;
using Couchbase.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;

namespace AzureFunctionsCouchbase
{
    public static class Function1
    {
        private static readonly Lazy<IBucket> Bucket = new Lazy<IBucket>(() =>
        {
            var uri = Environment.GetEnvironmentVariable("couchbaseUri");
            var bucketName = Environment.GetEnvironmentVariable("couchbaseBucketName");
            var username = Environment.GetEnvironmentVariable("couchbaseUsername");
            var password = Environment.GetEnvironmentVariable("couchbasePassword");
            var cluster = new Cluster(new ClientConfiguration
            {
                Servers = new List<Uri> { new Uri(uri) }
            });
            cluster.Authenticate(username, password);
            return cluster.OpenBucket(bucketName);
        });

        [FunctionName("GetDoc")]
        public static IActionResult GetDoc([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]HttpRequest req, TraceWriter log)
        {
            string id = req.Query["id"];

            var doc = Bucket.Value.Get<MyDocument>(id);

            return new OkObjectResult(doc.Value);
        }

        [FunctionName("SetDoc")]
        public static IActionResult SetDoc([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] MyDocument doc)
        {
            var id = Guid.NewGuid().ToString();

            Bucket.Value.Insert(id, doc);

            return new OkObjectResult($"New document inserted with ID {id}");
        }
    }

    public class MyDocument
    {
        public string Name { get; set; }
        public int ShoeSize { get; set; }
        public decimal Balance { get; set; }
    }
}

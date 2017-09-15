using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.Configuration.Client;
using Couchbase.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace CouchbaseWithAzureFunctions
{
    // tag::MyDocument[]
    public class MyDocument
    {
        public string Name { get; set; }
        public int ShoeSize { get; set; }
        public decimal Balance { get; set; }
    }
    // end::MyDocument[]

    public static class WriteToCouchbase
    {
        // tag::lazy[]
        private static readonly Lazy<IBucket> Bucket = new Lazy<IBucket>(() =>
        {
            var uri = ConfigurationManager.AppSettings["couchbaseUri"];
            var bucketName = ConfigurationManager.AppSettings["couchbaseBucketName"];
            var bucketPassword = ConfigurationManager.AppSettings["couchbaseBucketPassword"];
            var cluster = new Cluster(new ClientConfiguration
            {
                Servers = new List<Uri> { new Uri(uri) }
            });
            return cluster.OpenBucket(bucketName, bucketPassword);
        });
        // end::lazy[]

        [FunctionName("HttpTriggerCSharpGet")]
        public static async Task<HttpResponseMessage> Get([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            // parse query parameter
            var id = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "id", true) == 0)
                .Value;

            // tag::bucket[]
            var doc = await Bucket.Value.GetAsync<MyDocument>(id);
            // end::bucket[]
            return req.CreateResponse(HttpStatusCode.OK, doc.Value, new JsonMediaTypeFormatter());
        }

        [FunctionName("HttpTriggerCSharpSet")]
        public static async Task<HttpResponseMessage> Set([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] MyDocument req, TraceWriter log)
        {
            var id = Guid.NewGuid().ToString();

            await Bucket.Value.InsertAsync(id, req);

            return new HttpResponseMessage
            {
                Content = new StringContent($"New document inserted with ID {id}"),
                StatusCode = HttpStatusCode.OK
            };
        }
    }
}
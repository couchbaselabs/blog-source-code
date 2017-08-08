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
    public class MyDocument
    {
        public string Name { get; set; }
        public int ShoeSize { get; set; }
        public decimal Balance { get; set; }
    }

    public static class WriteToCouchbase
    {
        [FunctionName("HttpTriggerCSharpGet")]
        public static async Task<HttpResponseMessage> Get([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            // parse query parameter
            var id = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "id", true) == 0)
                .Value;

            using (var cluster = GetCluster())
            {
                using (var bucket = GetBucket(cluster))
                {
                    var doc = await bucket.GetAsync<MyDocument>(id);
                    return req.CreateResponse(HttpStatusCode.OK, doc.Value, new JsonMediaTypeFormatter());
                }
            }
        }

        [FunctionName("HttpTriggerCSharpSet")]
        public static async Task<HttpResponseMessage> Set([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] MyDocument req, TraceWriter log)
        {

            var id = Guid.NewGuid().ToString();

            using (var cluster = GetCluster())
            {
                //dynamic doc = await req.Content.ReadAsAsync<dynamic>();
                using (var bucket = GetBucket(cluster))
                {
                    await bucket.InsertAsync(id, req);
                }
            }

            return new HttpResponseMessage
            {
                Content = new StringContent($"New document inserted with ID {id}"),
                StatusCode = HttpStatusCode.OK
            };
        }

        private static IBucket GetBucket(Cluster cluster)
        {
            var bucketName = ConfigurationManager.AppSettings["couchbaseBucketName"];
            var bucketPassword = ConfigurationManager.AppSettings["couchbaseBucketPassword"];

            return cluster.OpenBucket(bucketName, bucketPassword);
        }

        private static Cluster GetCluster()
        {
            var uri = ConfigurationManager.AppSettings["couchbaseUri"];
            return new Cluster(new ClientConfiguration
            {
                Servers = new List<Uri> { new Uri(uri) }
            });
        }
    }
}
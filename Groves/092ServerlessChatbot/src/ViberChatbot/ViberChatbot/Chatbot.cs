using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.Configuration.Client;
using Couchbase.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace ViberChatbot
{
    public static class Chatbot
    {
        private static readonly Lazy<IBucket> Bucket = new Lazy<IBucket>(() =>
        {
            var uri = "uri for your couchbase server here";
            var bucketName = "ViberChatBot";
            var cluster = new Cluster(new ClientConfiguration
            {
                Servers = new List<Uri> { new Uri(uri) }
            });
            cluster.Authenticate("viberchatbot", "viberchatbot password here");
            return cluster.OpenBucket(bucketName);
        });

        // tag::Run[]
        [FunctionName("Chatbot")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req,
            TraceWriter log)
        {
            var incoming = req.Content.ReadAsAsync<ViberIncoming>().Result;

            var viber = new ViberProcessor(Bucket.Value);
            viber.Process(incoming);

            // return "OK" each time
            // this is most important for the initial Viber webhook setup
            return req.CreateResponse(HttpStatusCode.OK);
        }
        // end::Run[]
    }
}

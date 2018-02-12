using System;
using System.Collections.Generic;
using System.Configuration;
using Couchbase;
using Couchbase.Authentication;
using Couchbase.Configuration.Client;
using Couchbase.Core;

namespace AlexaBoothDuty.Core.IntentProcessors
{
    // this is to lazily load the couchbase cluster/bucket
    // when needed, using Lazy since I have no idea when azure
    // functions will spin up, parallelize, etc
    public static class CouchbaseBucket
    {
        public static IBucket GetBucket() => Bucket.Value;

        private static readonly Lazy<IBucket> Bucket = new Lazy<IBucket>(() =>
        {
            var uri = ConfigurationManager.AppSettings["couchbaseUri"];
            var bucketName = ConfigurationManager.AppSettings["couchbaseBucketName"];
            var username = ConfigurationManager.AppSettings["couchbaseUsername"];
            var password = ConfigurationManager.AppSettings["couchbasePassword"];
            var cluster = new Cluster(new ClientConfiguration
            {
                Servers = new List<Uri> { new Uri(uri) }
            });
            cluster.Authenticate(new PasswordAuthenticator(username, password));
            return cluster.OpenBucket(bucketName);
        });
    }
}
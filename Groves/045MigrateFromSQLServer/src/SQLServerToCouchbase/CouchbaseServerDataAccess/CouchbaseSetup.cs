using System;
using System.Collections.Generic;
using Couchbase;
using Couchbase.Configuration.Client;

namespace CouchbaseServerDataAccess
{
    public static class CouchbaseSetup
    {
        public static void SetupClusterHelper(string uri)
        {
            ClusterHelper.Initialize(new ClientConfiguration
            {
                Servers = new List<Uri> { new Uri(uri) }
            });
        }
    }
}
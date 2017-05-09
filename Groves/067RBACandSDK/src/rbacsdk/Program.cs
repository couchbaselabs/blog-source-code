using System;
using System.Collections.Generic;
using Couchbase;
using Couchbase.Authentication;
using Couchbase.Configuration.Client;
using Couchbase.Management;

namespace rbacsdk
{
    class Program
    {
        static void Main(string[] args)
        {
            var cluster = new Cluster(new ClientConfiguration
            {
                Servers = new List<Uri> {  new Uri("http://localhost:8091")},
                UseSsl = false
            });

            var clusterManager = cluster.CreateManager("Administrator", "password"); // cluster admin credentials

            // create bucket
            var resp = clusterManager.CreateBucket("testbucket", replicaNumber: 0);
            Console.WriteLine(resp.Success);
            Console.WriteLine(resp.Message);
            Console.WriteLine(resp.Exception?.Message);

            // create a user with only a data reader role
            var result = clusterManager.UpsertUser("matt", "mattspassword", "Matthew Groves", new Role { Name = "admin"});
            Console.WriteLine(result.Success);
            Console.WriteLine(result.Message);
            Console.WriteLine(result.Exception?.Message);

            // authenticate with that user
            var authenticator = new PasswordAuthenticator("matt", "mattspassword");
            cluster.Authenticate(authenticator);

            // get a bucket and try to write a document to it
            var bucket = cluster.OpenBucket("testbucket");

            var response = bucket.Insert(Guid.NewGuid().ToString(), new {foo = "barr"});

            Console.WriteLine(response.Success);
            Console.WriteLine(response.Message);
            Console.WriteLine(response.Exception?.Message);
        }
    }
}
using System;
using System.Collections.Generic;
using Couchbase;
using Couchbase.Collections;
using Couchbase.Configuration.Client;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ClusterHelper.Initialize(new ClientConfiguration {
                Servers = new List<Uri> { new Uri("couchbase://localhost")}
            });
            var bucket = ClusterHelper.GetBucket("data-structures");

            var list = new CouchbaseList<dynamic>(bucket, "myList");
            list.Add(new { foo = "bar" + Guid.NewGuid().ToString()});

            var map = new CouchbaseDictionary<string,string>(bucket, "myMap");
            map.Add("key" + Guid.NewGuid().ToString(), "value");

            ClusterHelper.Close();
        }
    }
}

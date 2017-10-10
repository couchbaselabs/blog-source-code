using System;
using System.Collections.Generic;
using System.Threading;
using Couchbase;
using Couchbase.Authentication;
using Couchbase.Configuration.Client;
using Couchbase.Core;

namespace FastFailoverDemo
{
    class Program
    {
        static IBucket _bucket;
        static bool terse = true;
        static int numDocuments = 50;

        static void Main(string[] args)
        {
            // tag::cluster[]
            var clientConfig = new ClientConfiguration
            {
                Servers = new List<Uri>
                {
                    new Uri("http://172.17.0.2"),
                    new Uri("http://172.17.0.3"),
                    new Uri("http://172.17.0.4")
                }
            };
            var cluster = new Cluster(clientConfig);

            var credentials = new PasswordAuthenticator("myuser", "password");
            cluster.Authenticate(credentials);

            _bucket = cluster.OpenBucket("mybucket");
            // end::cluster[]

            // tag::createdocs[]
            var docKeys = new List<string>();
            for (var i = 0; i < numDocuments; i++)
            {
                var key = "documentKey" + i;
                docKeys.Add(key);
                _bucket.Upsert(key, new { name = "Document" + i });
            }
            // end::createdocs[]

            // tag::mainloop[]
            var iteration = 0;
            while (true)
            {
                Console.WriteLine($"Getting {numDocuments} documents [{iteration++}]");
                foreach(var docKey in docKeys)
                {
                    var result = _bucket.Get<dynamic>(docKey);
                    if(terse)
                        ShowResultTerse(result, docKey);
                    else
                        ShowResult(result, docKey);
                }
                Console.WriteLine();

                Thread.Sleep(2000);
            }
            // end::mainloop[]
        }

        private static void ShowResultTerse(IOperationResult<dynamic> result, string id)
        {
            // happy path, document was found
            if (result.Success)
            {
                Console.Write("S");
                return;
            }

            // error, possibly node down
            // show error, try to get replica
            Console.Write(""); // don't write anything in terse version

            var replica = _bucket.GetFromReplica<dynamic>(id);

            // happy path for replica, it was found
            if (replica.Success)
            {
                Console.Write("R");
                return;
            }

            // error! replication may not be configured
            // or it's possible something catastrophic happened
            // this should be rare, but definitely want to log it
            // maybe retry and/or escalate
            // in this example, it's just logged to console
            Console.Write("F");
        }

        // tag::ShowResult[]
        private static void ShowResult(IOperationResult<dynamic> result, string id)
        {
            // happy path, document was found
            if (result.Success)
            {
                Console.WriteLine("Result: success");
                return;
            }

            // error, possibly node down
            // show error, try to get replica
            Console.WriteLine($"Result: unsuccessful {result.Message}");
            Console.WriteLine("\tAttempting to get replica.");

            var replica = _bucket.GetFromReplica<dynamic>(id);

            // happy path for replica, it was found
            if (replica.Success)
            {
                Console.WriteLine("\tReplica result: success");
                return;
            }

            // error! replication may not be configured
            // or it's possible something catastrophic happened
            // this should be rare, but definitely want to log it
            // maybe retry and/or escalate
            // in this example, it's just logged to console
            Console.WriteLine("\tReplica result unsuccessful: {result.Message}");
        }
        // end::ShowResult[]
    }
}

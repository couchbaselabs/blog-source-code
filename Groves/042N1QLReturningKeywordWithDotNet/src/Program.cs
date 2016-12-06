using System;
using System.Collections.Generic;
using Couchbase;
using Couchbase.Configuration.Client;
using Couchbase.N1QL;
using Newtonsoft.Json;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // connect to couchbase cluster and get a bucket
            // this example assumes a couchbase node running locally
            // this example assumes a bucket exists called 'default
            var client = new ClientConfiguration {
                Servers = new List<Uri> { new Uri("couchbase://localhost") }
            };
            ClusterHelper.Initialize(client);
            var bucket = ClusterHelper.GetBucket("default");

            // create some documents that *don't* have a 'processed' field
            // tag::createdocuments[]
            for(var i = 0;i < 5; i++) {
                var docKey = Guid.NewGuid().ToString();
                var docContent = new {
                        foo = "bar",
                        type = "example",
                        processed = false,
                        dt = DateTime.Now
                };
                var docContentJson = JsonConvert.SerializeObject(docContent);
                bucket.Insert(new Document<dynamic> {
                    Id = docKey,
                    Content = docContent 
                });

                Console.WriteLine($"Inserted: {docKey} - {docContentJson}");
            }
            // end::createdocuments[]

            Console.WriteLine();

            // update all 'example' documents, setting a 'processed' field to true
            // and use RETURNING keyword to get the documents that were processed
            // Note that you need (at least) a primary key index on the default
            // tag::updatereturning[]
            var n1ql = @"UPDATE `default` d
                            SET processed = true
                            WHERE d.type = 'example'
                            AND d.processed = false
                            RETURNING d.*, META().id AS docKey";
            var query = QueryRequest.Create(n1ql);
            query.ScanConsistency(ScanConsistency.RequestPlus);
            var results = bucket.Query<dynamic>(query);
            // end::updatereturning[]

            if(!results.Success) {
                Console.WriteLine("There was an error.");
                Console.WriteLine(results.Message);
                results.Errors.ForEach(e => Console.WriteLine(e.Message));
            }

            // the results should be the same five documents created initially (but with processed fields)
            // tag::showreturning[]
            foreach(var result in results.Rows) {
                var resultJson = JsonConvert.SerializeObject(result);
                Console.WriteLine($"Returned: {resultJson}");
            }
            // end::showreturning[]
        }
    }
}

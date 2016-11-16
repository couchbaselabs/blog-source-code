// tag::namespaces[]
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.Configuration.Client;
using Couchbase.N1QL;
// end::namespaces[]

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // tag::connect[]
            // connect to a cluster, get a bucket
            ClusterHelper.Initialize(new ClientConfiguration {
                Servers = new List<Uri> { new Uri("couchbase://localhost")}
            });
            var bucket = ClusterHelper.GetBucket("default");
            // end::connect[]

            // tag::insert[]
            // insert a document with some random values
            var document = new Document<dynamic> {
                Id = Guid.NewGuid().ToString(),
                Content = new {
                    invoiceNumber = Path.GetRandomFileName(),
                    amountDue = new Random().Next(10,300),
                    type = "electricbill"
                }
            };
            Task.WaitAll(bucket.InsertAsync(document)); // wait for async method to finish
            Console.WriteLine("New electric bill created.");
            Console.WriteLine();
            // end::insert[]

            // tag::n1ql[]
            // get all electric bills with N1QL and list them
            var query = Couchbase.N1QL.QueryRequest.Create("SELECT b.* FROM `default` b WHERE type = $1");
            query.AddPositionalParameter("electricbill");
            query.ScanConsistency(ScanConsistency.RequestPlus);
            var result = bucket.QueryAsync<dynamic>(query).Result;  // calling QueryAsync synchronously
            // end::n1ql[]

            // tag::n1qlresults[]
            Console.WriteLine("Success: " + result.Success);
            if(!result.Success) {
                Console.WriteLine("Message: " + result.Message);
                Console.WriteLine("Exception: " + result.Exception?.GetType().Name);
                Console.WriteLine("Exception Message: " + result?.Exception?.Message);
                result.Errors.ForEach(e => Console.WriteLine("Error: " + e?.Message));
                return;
            }

            Console.WriteLine();
            Console.WriteLine("Bills:");
            Console.WriteLine("------");
            foreach(var bill in result.Rows) {
                Console.WriteLine($"{bill.invoiceNumber} - {bill.amountDue.ToString("C")}");
            }
            // end::n1qlresults[]
        }
    }
}

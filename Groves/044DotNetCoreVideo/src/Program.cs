using System;
using System.Collections.Generic;
using System.IO;
using Couchbase;
using Couchbase.Configuration.Client;
using Couchbase.N1QL;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ClusterHelper.Initialize(new ClientConfiguration {
                Servers = new List<Uri> { new Uri("couchbase://localhost")}
            });
            var bucket = ClusterHelper.GetBucket("default");

            var doc = new Document<dynamic> {
                Id = Guid.NewGuid().ToString(),
                Content = new {
                    invoiceNumber = Path.GetRandomFileName(),
                    amountDue = new Random().Next(10,1000),
                    type = "electric_bill"
                }
            };
            bucket.Insert(doc);

            // --------

            var query = QueryRequest.Create("SELECT b.* FROM `default` b WHERE b.type = $1");
            query.AddPositionalParameter("electric_bill");
            query.ScanConsistency(ScanConsistency.RequestPlus);
            var result = bucket.Query<dynamic>(query);

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
            Console.WriteLine("---------");
            foreach(var bill in result.Rows) {
                Console.WriteLine($"{bill.invoiceNumber} - {bill.amountDue.ToString("C")}");
            }
        }
    }
}

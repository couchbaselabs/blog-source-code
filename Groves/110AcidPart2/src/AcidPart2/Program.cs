using System;
using System.Collections.Generic;
using Couchbase;
using Couchbase.Configuration.Client;

namespace AcidPart2
{
    class Program
    {
        static void Main(string[] args)
        {
            // connect to couchbase
            var cluster = new Cluster(new ClientConfiguration
            {
                Servers = new List<Uri> { new Uri("http://localhost:8091") }
            });
            cluster.Authenticate("matt", "password");
            var bucket = cluster.OpenBucket("farm");

            // create two randomized Barn documents
            var random = new Random();
            var barn1Key = "barn::" + Guid.NewGuid();
            var barn2Key = "barn::" + Guid.NewGuid();
            bucket.Insert(barn1Key, new Barn { Name = Faker.NameFaker.LastName() + " Barn", Chickens = random.Next(1, 30) });
            bucket.Insert(barn2Key, new Barn { Name = Faker.NameFaker.LastName() + " Barn", Chickens = random.Next(1, 30) });

            // and immediately retrieve them (and display to UI)
            var barn1 = bucket.Get<Barn>(barn1Key, TimeSpan.FromSeconds(30));
            var barn2 = bucket.Get<Barn>(barn2Key, TimeSpan.FromSeconds(30));
            Console.WriteLine($"{barn1.Value.Name} has {barn1.Value.Chickens} chickens.");
            Console.WriteLine($"{barn2.Value.Name} has {barn2.Value.Chickens} chickens.");
            Console.ReadLine();

            // announce that a transaction will happen
            var amountToTransfer = 1;
            Console.WriteLine($"Transferring {amountToTransfer} chickens from first barn to second.");
            Console.ReadLine();

            // change the 'false,false' to simulate errors to trigger rollback
            var transactionHelper = new TransactionHelper(bucket, false, false);
            transactionHelper.Perform(barn1, barn2, amountToTransfer);

            cluster.Dispose();
        }
    }
}

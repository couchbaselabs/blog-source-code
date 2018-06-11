using System;
using System.Collections.Generic;
using System.Threading;
using Couchbase;
using Couchbase.Configuration.Client;
using Couchbase.Core;

namespace SimulateTickets
{
    class Program
    {
        private static IBucket _bucket;

        static void Main(string[] args)
        {
            var couchbaseUrl = "http://localhost:8091";
            var couchbaseUsername = "knowidemo";
            var couchbasePassword = "password";
            var couchbaseBucketName = "tickets";

            var cluster = new Cluster(new ClientConfiguration
            {
                Servers = new List<Uri> {new Uri(couchbaseUrl) }
            });
            cluster.Authenticate(couchbaseUsername, couchbasePassword);
            _bucket = cluster.OpenBucket(couchbaseBucketName);

            // these coordinates correspond to areas in and around Great American Ball Park
            var thirdBaseSide = (topLeftLat: 39098049, topLeftLon: -84507112, bottomRightLat: 39097681, bottomRightLon: -84505854);
            var firstBaseSide = (topLeftLat: 39097599, topLeftLon: -84507692, bottomRightLat: 39096850, bottomRightLon: -84507154);
            var random = new Random();

            // simulate the entry of between 50 and 100 ticket holders every 2 seconds
            while (true)
            {
                var numTicketsScanned = random.Next(50, 100);
                for (int i = 0; i < numTicketsScanned; i++)
                {
                    var coinFlip = random.Next(0, 500) > 250;
                    if (coinFlip)
                    {
                        var lat = random.Next(thirdBaseSide.bottomRightLat, thirdBaseSide.topLeftLat) / 1000000.0;
                        var lon = random.Next(thirdBaseSide.topLeftLon, thirdBaseSide.bottomRightLon) / 1000000.0;
                        Insert(lat, lon);
                    }
                    else
                    {
                        var lat = random.Next(firstBaseSide.bottomRightLat, firstBaseSide.topLeftLat) / 1000000.0;
                        var lon = random.Next(firstBaseSide.topLeftLon, firstBaseSide.bottomRightLon) / 1000000.0;
                        Insert(lat, lon);
                    }
                }

                Console.WriteLine($"{numTicketsScanned} checked in.");

                // wait 2 seconds to scan any more tickets
                Thread.Sleep(2000);
            }
        }

        private static void Insert(double lat, double lon)
        {
            _bucket.Insert("ticket::" + Guid.NewGuid(), new
            {
                Name = Faker.NameFaker.Name(),
                Geo = new
                {
                    lat = lat,
                    lon = lon
                },
                Type = "ticketScan"
            });
        }
    }
}

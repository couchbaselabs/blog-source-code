using System;
using System.Collections.Generic;
using System.IO;
using Couchbase;
using Couchbase.Configuration.Client;
using Couchbase.Core;
using Newtonsoft.Json;

namespace TicketVip
{
    class Program
    {
        private static IBucket _bucket;
        private static Random _rand;
        private static string _yourVerifiedNumber = "< your number here >"; // with Twilio trial account you can only send to verified phone numbers

        static void Main(string[] args)
        {
            var cluster = new Cluster(new ClientConfiguration
            {
                Servers = new List<Uri> {new Uri("http://localhost:8091")}
            });
            cluster.Authenticate("matt", "password");
            _bucket = cluster.OpenBucket("tickets");

            _rand = new Random();

            // *** these functions are setup code, they only really need to run the first time
            PopulateVips();
            PopulateVipConcierges();
            LoadTwilioCredentials();
            // ***

            while (true)
            {
                // tag::consoleapp[]
                Console.WriteLine("1 - Simulate a VIP ticket scan.");
                Console.WriteLine("2 - Simulate a regular joe ticket scan.");
                Console.WriteLine("Q - End simulation.");
                var choice = Console.ReadKey().KeyChar;
                // end::consoleapp[]
                Console.WriteLine();
                switch (choice)
                {
                    case '1':
                        SimulateTicketScan(isVip: true);
                        break;
                    case '2':
                        SimulateTicketScan(isVip: false);
                        break;
                    case 'q': case 'Q':
                        return;
                }
                Console.WriteLine();
                Console.WriteLine();
            }
        }

        private static void SimulateTicketScan(bool isVip)
        {
            // create some random data to simulate a ticket scan
            var ticketScanId = "ticketscan::" + Guid.NewGuid();
            var ticketScanTimestamp = DateTime.Now;
            var randomSection = _rand.Next(398, 449);
            var randomRow = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".Substring(_rand.Next(0,26),1);
            var randomSeat = _rand.Next(1, 19);
            var seatInformation = $"Section {randomSection} Row {randomRow} Seat {randomSeat}";
            var customerId = "customer::";
            if (isVip)
                customerId += _rand.Next(1, 10);
            else
                customerId += _rand.Next(11, 45000);

            // put the ticket scan document into Couchbase
            // tag::ticketscan[]
            _bucket.Upsert(ticketScanId, new {CustomerId = customerId, Timestamp = ticketScanTimestamp, Seat = seatInformation });
            // end::ticketscan[]

            Console.WriteLine($"Ticket id '{ticketScanId}' was scanned at '{ticketScanTimestamp}'.");
            if(isVip)
                Console.WriteLine("\t(This is a VIP)");
        }

        // this function will create a number of VIPs if they don't already exist
        // note that if you change the data, you'll need to change SimulateTicketScan
        private static void PopulateVips()
        {
            // tag::vips[]
            _bucket.Upsert("customer::1", new { Name = "George Clooney" });
            _bucket.Upsert("customer::2", new { Name = "Josh Hutcherson" });
            _bucket.Upsert("customer::3", new { Name = "Darius Rucker" });
            _bucket.Upsert("customer::4", new { Name = "Brooklyn Decker" });
            _bucket.Upsert("customer::5", new { Name = "Eddie Vedder" });
            _bucket.Upsert("customer::6", new { Name = "Nick Lachey" });
            _bucket.Upsert("customer::7", new { Name = "Nick Goepper" });
            _bucket.Upsert("customer::8", new { Name = "Johnny Bench" });
            _bucket.Upsert("customer::9", new { Name = "Ryan Collins" });
            // end::vips[]
        }

        // this function will create a number of VIP concierges if they don't already exist
        private static void PopulateVipConcierges()
        {
            // tag::concierges[]
            _bucket.Upsert("concierge::1", new
            {
                Name = "Matt Groves",
                CellNumber = _yourVerifiedNumber,
                vips = new List<string> { "customer::1", "customer::2", "customer::9" }
            });
            _bucket.Upsert("concierge::2", new
            {
                Name = "Mr. Redlegs",
                CellNumber = _yourVerifiedNumber,
                vips = new List<string> { "customer::3", "customer::4", "customer::5" }
            });
            _bucket.Upsert("concierge::3", new
            {
                Name = "Rosie Red",
                CellNumber = _yourVerifiedNumber,
                vips = new List<string> { "customer::6", "customer::7", "customer::8" }
            });
            // end::concierges[]
        }

        // this function will create a document containing twilio credentials
        // this isn't required, but it makes it easier for me to develop and put on github
        // you will need to create a twiliocredentials.json containing your own credentials
        private static void LoadTwilioCredentials()
        {
            if (!File.Exists("twiliocredentials.json"))
            {
                Console.WriteLine("You must create a twiliocredentials.json file for this demo. Check out twiliocredentials.template.json for a guide.");
                Environment.Exit(0);
            }
            var json = File.ReadAllText("twiliocredentials.json");
            var obj = JsonConvert.DeserializeObject(json);
            _bucket.Upsert("twilio::credentials", obj);
        }
    }
}

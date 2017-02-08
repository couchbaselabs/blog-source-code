using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Couchbase;
using Couchbase.Configuration.Client;
using Couchbase.Core;
using SQLServerDataAccess;

namespace SQLServerToCouchbase.Migrate
{
    class Program
    {
        static IBucket _bucket;
        static SqlToCbContext _context;

        static void Main(string[] args)
        {
            SetupConnections();

            var cartMigrator = new ShoppingCartMigrator(_bucket, _context);
            if (!cartMigrator.Go())
            {
                Console.WriteLine("Error migrating Shopping Carts. Rolling back entire migration.");
                cartMigrator.Rollback();
            }

            var socialMediaMigrator = new SocialMediaMigrator(_bucket, _context);
            if (!socialMediaMigrator.Go())
            {
                Console.WriteLine("Error migrating Social Media. Rolling back entire migration.");
                socialMediaMigrator.Rollback();
            }
        }

        private static void SetupConnections()
        {
            ClusterHelper.Initialize(new ClientConfiguration
            {
                Servers = new List<Uri> {  new Uri("couchbase://localhost")}
            });
            _bucket = ClusterHelper.GetBucket("sqltocb");

            var db = new SqlConnection("data source=(local);initial catalog=sqltocb;integrated security=True;");
            _context = new SqlToCbContext(db);
        }
    }
}

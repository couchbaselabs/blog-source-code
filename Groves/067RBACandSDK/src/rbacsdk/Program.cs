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
            ShowAuthenticationFail();

            Console.WriteLine();
            Console.WriteLine("--------------");
            Console.WriteLine();

            ShowAuthorizationFail();

            Console.WriteLine();
            Console.WriteLine("Press a key to continue...");
            Console.ReadKey();
        }

        private static void ShowAuthorizationFail()
        {
            // authenticate and get a bucket
            // tag::authandinsert[]
            var cluster = new Cluster(new ClientConfiguration
            {
                Servers = new List<Uri> { new Uri("http://localhost:8091") }
            });
            var authenticator = new PasswordAuthenticator("myuser", "password");
            cluster.Authenticate(authenticator);
            var bucket = cluster.OpenBucket("mybucket");

            // insert a document, this should be allowed
            var result = bucket.Insert(Guid.NewGuid().ToString(), new {foo = "bar"});
            Console.WriteLine("Insert was successful: " + result.Success);
            // end::authandinsert[]

            // execute a query, this should not be allowed
            // tag::noauthforn1ql[]
            var queryResult = bucket.Query<int>("SELECT COUNT(1) FROM `" + bucket.Name + "`");
            Console.WriteLine("Query was successful: " + queryResult.Success);
            queryResult.Errors.ForEach(e => Console.WriteLine("Error: " + e.Message));
            // end::noauthforn1ql[]
        }

        private static void ShowAuthenticationFail()
        {
            // tag::createcluster[]
            var cluster = new Cluster(new ClientConfiguration
            {
                Servers = new List<Uri> { new Uri("http://localhost:8091") }
            });
            // end::createcluster[]

            // use an incorrect password
            // tag::incorrectcreds[]
            var authenticator = new PasswordAuthenticator("myuser", "wrongpassword");
            cluster.Authenticate(authenticator);
            // end::incorrectcreds[]

            // try to get a bucket
            // tag::authexception[]
            try
            {
                var bucket = cluster.OpenBucket("mybucket");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting bucket.");
                Console.WriteLine(ex.Message);
            }
            // end::authexception[]
        }
    }
}
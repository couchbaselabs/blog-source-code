using System;
using System.Collections.Generic;
using Couchbase;
using Couchbase.Configuration.Client;
using Couchbase.Core;
using Couchbase.IO;
using Newtonsoft.Json;

namespace PartialReadExample
{
    class Program
    {
        private static IBucket _bucket;
        private static readonly Random _random = new Random();

        static void Main(string[] args)
        {
            SetupCouchbase();

            Bug();

//            var key = "Profile::" + _random.Next(10000);
//            InsertADocument(key);
//
//            // tag::WholeDocument[]
//            var wholeDocument = _bucket.Get<dynamic>(key).Value;
//            Console.WriteLine($"===Whole document (Key = {key})");
//            Console.WriteLine(JsonConvert.SerializeObject(wholeDocument,Formatting.Indented));
//            // end::WholeDocument[]
//            Console.WriteLine();
//
//            // tag::TryShowSubDocument[]
//            ShowSubdocument(key, "username");
//            ShowSubdocument(key, "profile");
//            ShowSubdocument(key, "profile.phoneNumber");
//            ShowSubdocument(key, "profile.address.street");
//            ShowSubdocument(key, "profile.address.province");
//            // end::TryShowSubDocument[]
//
//            // tag::TryDoesSubdocumentExist[]
//            DoesSubdocumentExist(key, "profile.address.state");
//            DoesSubdocumentExist(key, "profile.address.province");
//            // end::TryDoesSubdocumentExist[]

            CleanupCouchbase();
        }

        private static void Bug()
        {
            var doc = new Document<dynamic>
            {
                Id = "Foo::123",
                Content = new
                {
                    Username = "mgroves",
                    Profile = new
                    {
                        PhoneNumber = "123-456-7890",
                        Address = new
                        {
                            Street = "123 Main Rd",
                            City = "Columbus",
                            State = "Ohio"
                        }
                    }
                }
            };
            _bucket.Upsert(doc);

            var subDoc = _bucket.LookupIn<dynamic>("Foo::123").Exists("profile.address.state").Execute();
            Console.WriteLine("Subdoc profile.address.state exists: " + subDoc.Exists("profile.address.state"));

            var subDoc2 = _bucket.LookupIn<dynamic>("Foo::123").Exists("profile.address.province").Execute();
            Console.WriteLine("Subdoc profile.address.province exists: " + subDoc2.Exists("profile.address.province"));
        }

        // tag::DoesSubdocumentExist[]
        private static void DoesSubdocumentExist(string key, string path)
        {
            Console.WriteLine($"== Checking if path '{path}' exists in document '{key}'");
            var subDoc = _bucket.LookupIn<dynamic>(key).Exists(path).Execute();

            Console.WriteLine(subDoc.Status == ResponseStatus.Success
                ? $"SubDocument '{path}' exists"
                : $"SubDocument '{path}' doesn't exist");
            Console.WriteLine();
        }
        // end::DoesSubdocumentExist[]

        // tag::ShowSubdocument[]
        private static void ShowSubdocument(string key, string path)
        {
            var subDoc = _bucket.LookupIn<dynamic>(key).Get(path).Execute();
            Console.WriteLine($"===Subdocument ({path})");

            if (subDoc.Status == ResponseStatus.Success)
                Console.WriteLine(JsonConvert.SerializeObject(subDoc.Content(path), Formatting.Indented));
            else
                Console.WriteLine(subDoc.Status);

            Console.WriteLine();
        }
        // end::ShowSubdocument[]

        private static void InsertADocument(string key)
        {
            var doc = new Document<dynamic>
            {
                Id = key,
                Content = new
                {
                    Username = "mgroves",
                    Profile = new
                    {
                        PhoneNumber = "123-456-7890",
                        Address = new
                        {
                            Street = "123 Main Rd",
                            City = "Columbus",
                            State = "Ohio"
                        }
                    }
                }
            };
            _bucket.Insert(doc);
        }

        private static void CleanupCouchbase()
        {
            ClusterHelper.Close();
        }

        private static void SetupCouchbase()
        {
            var config = new ClientConfiguration();
            config.Servers = new List<Uri>
            {
                new Uri("http://localhost:8091")
            };

            config.UseSsl = false;
            ClusterHelper.Initialize(config);
            _bucket = ClusterHelper.GetBucket("travel-sample");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Couchbase.Configuration.Client;
using Couchbase.Core;
using Couchbase.IO;

namespace Couchbase.Examples.SubDocumentAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            ClusterHelper.Initialize(new ClientConfiguration
            {
                Servers = new List<Uri>
                {
                    new Uri(ConfigurationManager.AppSettings["couchbaseUri"])
                }
            });

            var bucket = ClusterHelper.GetBucket("default");
            const string id = "puppy";
            var dog = new
            {
                Type = "dog",
                Breed = "Pitbull/Chihuahua",
                Name = "Puppy",
                Toys = new List<string> {"squeaker", "ball", "shoe"},
                Owner = new
                {
                    Type = "servant",
                    Name = "Don Knotts",
                    Age = 63
                },
                Attributes = new Dictionary<string, object>
                {
                    {"Fleas", true},
                    {"Color", "white"},
                    {"EyeColor", "brown"},
                    {"Age", 5},
                    {"Dirty", true },
                    {"Sex", "female" }
                },
                Counts = new List<object> { 1}
            };

            Reset(bucket, id, dog);
            LookupInChaining(bucket, id);

            Reset(bucket, id, dog);
            ErrorExample(bucket, id);

            Reset(bucket, id, dog);
            GetExample(bucket, "owner", id);

            Reset(bucket, id, dog);
            InsertExample(bucket, id,"attributes.hairLength", "short");

            Reset(bucket, id, dog);
            InsertExample(bucket, id, "anewattribute.withakey", "somevalue");

            Reset(bucket, id, dog);
            RemoveExample(bucket, id, "owner.name");

            Reset(bucket, id, dog);
            ReplaceExample(bucket, id, "owner", new { CatLover=true, CatName="celia"});

            Reset(bucket, id, dog);
            PushBackExample(bucket, id, "toys", "slipper");

            Reset(bucket, id, dog);
            PushFrontExample(bucket, id, "toys", "slipper");

            Reset(bucket, id, dog);
            ArrayInsertExample(bucket, id, "toys[2]", "slipper");

            Reset(bucket, id, dog);
            AddUniqueExample(bucket, id, "toys", "shoe");

            Reset(bucket, id, dog);
            AddUniqueExample(bucket, id, "counts", "1");

            Reset(bucket, id, dog);
            CounterExample(bucket, id, "likes", 1);

            Reset(bucket, id, dog);
            CounterExample(bucket, id, "likes", -1);

            ClusterHelper.Close();
            Console.Read();
        }

        static void Reset(IBucket bucket, string id, dynamic doc)
        {
            var result = bucket.Upsert(new Document<dynamic>
            {
                Id = id,
                Content = doc
            });
            Console.WriteLine("Updated {0} - {1}", id, result.Status);
        }

        public static void LookupInChaining(IBucket bucket, string id)
        {
           var builder = bucket.LookupIn(id).
                Get("type").
                Get("name").
                Get("owner").
                Exists("notfound");

            var fragment = builder.Execute<dynamic>();

            if (fragment.OpStatus("type") == ResponseStatus.Success)
            {
                string format = "Path='{0}' Value='{1}'";
                Console.WriteLine(format, "type", fragment.Content("type"));
            }
            GetDisplay(fragment, "type");
            GetDisplay(fragment, "name");
            GetDisplay(fragment, "owner");
        }

        static void GetDisplay<T>(IDocumentFragment<T> fragment, string path)
        {
            string format = "Path='{0}' Value='{1}'";
            Console.WriteLine(format, path, fragment.Content(path));
        }

        public static void ErrorExample(IBucket bucket, string id)
        {
            var builder = bucket.LookupIn(id).
                Get("type").
                Get("somepaththatdoesntexist").
                Get("owner");

            var fragment = builder.Execute<dynamic>();
            Console.WriteLine("Generic error: {0}{1}Specific Error: {2}", 
                fragment.Status, Environment.NewLine, fragment.OpStatus(1));

            Console.WriteLine("Generic error: {0}{1}Specific Error: {2}",
               fragment.Status, Environment.NewLine, fragment.OpStatus("somepaththatdoesntexist"));
        }

        public static void GetExample(IBucket bucket, string path, string id)
        {
            var builder = bucket.LookupIn(id).
                Get(path).
                Execute<dynamic>();

            var fragment = builder.Content(path);
            Console.WriteLine(fragment);
        }

        public static void ExistsExample(IBucket bucket, string path, string id)
        {
            var builder = bucket.LookupIn(id).
                Exists(path).
                Execute<dynamic>();

            var found = builder.Content(path);
            Console.WriteLine(found);
        }

        public static void InsertExample(IBucket bucket, string id, string path, string value)
        {
            var fragment = bucket.MutateIn(id).
                Insert(path, value, false).
                Execute<dynamic>();

            var status = fragment.OpStatus(path);
            Console.WriteLine(status);
        }

        public static void RemoveExample(IBucket bucket, string id, string path)
        {
            var fragment = bucket.MutateIn(id).
                Remove(path).
                Execute<dynamic>();

            var status = fragment.OpStatus(path);
            Console.WriteLine(status);
        }

        public static void ReplaceExample(IBucket bucket, string id, string path, object value)
        {
            var fragment = bucket.MutateIn(id).
                Replace(path, value).
                Execute<dynamic>();

            var status = fragment.OpStatus(path);
            Console.WriteLine(status);
        }

        public static void PushBackExample(IBucket bucket, string id, string path, object value)
        {
            var fragment = bucket.MutateIn(id).
                PushBack(path, value, false).
                Execute<dynamic>();

            var status = fragment.OpStatus(path);
            Console.WriteLine(status);
        }

        public static void PushFrontExample(IBucket bucket, string id, string path, object value)
        {
            var fragment = bucket.MutateIn(id).
                PushFront(path, value, false).
                Execute<dynamic>();

            var status = fragment.OpStatus(path);
            Console.WriteLine(status);
        }

        public static void ArrayInsertExample(IBucket bucket, string id, string path, object value)
        {
            var fragment = bucket.MutateIn(id).
                ArrayInsert(path, value).
                Execute<dynamic>();

            var status = fragment.OpStatus(path);
            Console.WriteLine(status);
        }

        public static void AddUniqueExample(IBucket bucket, string id, string path, object value)
        {
            var fragment = bucket.MutateIn(id).
                AddUnique(path, value).
                Execute<dynamic>();

            var status = fragment.OpStatus(path);
            Console.WriteLine(status);
        }

        public static void CounterExample(IBucket bucket, string id, string path, long delta)
        {
            var fragment = bucket.MutateIn(id).
                Counter(path, delta).
                Execute<dynamic>();

            var status = fragment.OpStatus(path);
            Console.WriteLine(status);
        }
    }
}

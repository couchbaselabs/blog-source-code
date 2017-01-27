using System;
using System.Collections.Generic;
using Couchbase;
using Couchbase.Collections;
using Couchbase.Configuration.Client;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ClusterHelper.Initialize(new ClientConfiguration {
                Servers = new List<Uri> { new Uri("couchbase://localhost")}
            });
            var bucket = ClusterHelper.GetBucket("data-structures");

            // creates list document or uses existing list document
            // tag::createlist[]
            var list = new CouchbaseList<dynamic>(bucket, "myList");
            // end::createlist[]

            // tag::examplesoflist[]
            // add 10 objects to the list
            for(var i = 0; i < 10; i++)
                list.Add(new { num = i, foo = "bar" + Guid.NewGuid()});

            // remove an item from the list by index
            list.RemoveAt(5);

            // show an item from the list by index
            var item = list[5];
            Console.WriteLine("6th item in the list: " + item.foo + " / " + item.num);
            // end::examplesoflist[]

            // --- queue
            Console.WriteLine();

            // creates queue document or uses existing queue document
            // tag::createqueue[]
            var queue = new CouchbaseQueue<dynamic>(bucket, "myQueue");
            // end::createqueue[]

            // tag::queueexample[]
            for(var i = 0; i < 3; i++)
                queue.Enqueue(new { num = i, foo = "baz" + Guid.NewGuid()});

            // dequeue
            var item = queue.Dequeue();
            Console.WriteLine("item num " + item.num + " was dequeued. There are now " + queue.Count + " items left in the queue.");
            // end::queueexample[]

            // --- dictionary
            Console.WriteLine();

            // creates dictionary document or uses existing dictionary document
            // tag::createdict[]
            var dict = new CouchbaseDictionary<string,dynamic>(bucket, "myDict");
            // end::createdict[]
            // add 5 k/v pairs to the dictionary
            // tag::dictexample[]
            for(var i = 0; i < 5; i++)
                dict.Add("key" + Guid.NewGuid(), new { num = i, foo = "qux" + Guid.NewGuid()} );

            // print out keys in the dictionary
            Console.WriteLine("There are " + dict.Keys.Count + " keys in the dictionary.");
            foreach(var key in dict.Keys)
                Console.WriteLine("key: " + key + ", value.num: " + dict[key].num);
            // end::dictexample[]

            ClusterHelper.Close();
        }
    }
}

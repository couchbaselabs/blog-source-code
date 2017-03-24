using Couchbase;
using Couchbase.Collections;
using Couchbase.Configuration.Client;
using Couchbase.Core;
using System;
using System.Collections.Generic;

namespace DataStructures
{
    class Program
    {
        static void Main(string[] args)
        {
            ClusterHelper.Initialize(new ClientConfiguration
            {
                Servers = new List<Uri> { new Uri("couchbase://localhost")}
            });
            var bucket = ClusterHelper.GetBucket("default");

            //ListDemo(bucket);

            //QueueDemo(bucket);

            DictionaryDemo(bucket);
        }





        private static void ListDemo(IBucket bucket)
        {
            //var pizzaPlaces = new List<PizzaPlace>();
            var pizzaPlaces = new CouchbaseList<PizzaPlace>(bucket, "List_PizzaPlaces");
            pizzaPlaces.Clear();

            pizzaPlaces.Add(new PizzaPlace { Name = "Tammy's Pizza", Rating = 5, Delivery = false });
            pizzaPlaces.Add(new PizzaPlace { Name = "Flyer's", Rating = 3, Delivery = true });

            foreach (var pizzaPlace in pizzaPlaces)
                Console.WriteLine(pizzaPlace.Name);
            Console.WriteLine();

            pizzaPlaces.Add(new PizzaPlace { Name = "Reggio", Rating = 4, Delivery = false });

            foreach (var pizzaPlace in pizzaPlaces)
                Console.WriteLine(pizzaPlace.Name);
        }





        private static void QueueDemo(IBucket bucket)
        {
            //var orders = new Queue<PizzaOrder>();
            var orders = new CouchbaseQueue<PizzaOrder>(bucket, "Queue_PizzaOrders");
            orders.Clear();

            orders.Enqueue(new PizzaOrder { Name = "Matt", OrderNumber = 1, OrderTime = DateTime.Now.AddMinutes(-10) });
            orders.Enqueue(new PizzaOrder { Name = "Ali", OrderNumber = 2, OrderTime = DateTime.Now.AddMinutes(-5) });

            var order1 = orders.Dequeue();
            Console.WriteLine($"Order #{order1.OrderNumber} completed for: {order1.Name}");

            orders.Enqueue(new PizzaOrder { Name = "Caesar", OrderNumber = 3, OrderTime = DateTime.Now });

            Console.WriteLine($"There are {orders.Count} orders left in the queue.");
        }






        private static void DictionaryDemo(IBucket bucket)
        {
            //var dictionary = new Dictionary<string, PizzaEmployee>();
            var dictionary = new CouchbaseDictionary<string, PizzaEmployee>(bucket, "Dictionary_PizzaStaff");
            dictionary.Clear();

            dictionary.Add("Matt", new PizzaEmployee { Shift = "8-5", HourlyWage = 7.25M });
            dictionary.Add("Mary", new PizzaEmployee { Shift = "5-12", HourlyWage = 8.25M });

            foreach (var key in dictionary.Keys)
                Console.WriteLine($"Employee '{key}' makes {dictionary[key].HourlyWage}/hour");
            Console.WriteLine();

            dictionary.Add("Hiro", new PizzaEmployee { Shift = "5-12", HourlyWage = 10.25M });

            try
            {
                dictionary.Add("Matt", new PizzaEmployee { Shift = "8-12", HourlyWage = 20.00M });
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Can't add 'Matt' to the dictionary twice.");
                Console.WriteLine();
            }

            foreach (var key in dictionary.Keys)
                Console.WriteLine($"Employee '{key}' makes {dictionary[key].HourlyWage}/hour");
        }
    }
}
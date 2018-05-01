using System;
using System.Collections.Generic;
using System.Threading;
using Couchbase;
using Couchbase.Configuration.Client;
using Couchbase.Core;

namespace AcidPart1
{
    class Program
    {
        static void Main(string[] args)
        {
            // couchbase cluster connection and bucket
            var cluster = new Cluster(new ClientConfiguration
            {
                Servers = new List<Uri> {new Uri("http://localhost:8091")}
            });
            cluster.Authenticate("matt", "password");
            var bucket = cluster.OpenBucket("mybucket");

            //OptimisticLockingExample(bucket);
            //PessimisticLockingExample1(bucket);
            //PessimisticLockingExample2(bucket);
            DurabilityExample(bucket);

            cluster.Dispose();
        }

        private static void DurabilityExample(IBucket bucket)
        {
            // tag::default[]
            // default memory-first
            var result1 = bucket.Upsert("memory-first", new {twitter = "@mgroves"});
            if(result1.Success)
                Console.WriteLine("Success");
            // end::default[]

            // tag::justreplicate[]
            // replicate
            var result2 = bucket.Upsert("replicate-to-1", new { email = "matthew.groves@couchbase.com" }, ReplicateTo.One);
            if (!result2.Success)
                Console.WriteLine("This will also fail if I only have 1 node");
            // end::justreplicate[]

            // tag::both[]
            // persist and replicate
            var result3 = bucket.Upsert("replicate-to-1-persist-to-1", new { site = "blog.couchbase.com" }, ReplicateTo.One, PersistTo.One);
            if (!result3.Success)
                Console.WriteLine("This will also fail if I only have 1 node");
            // end::both[]

            // etc...
        }

        private static void PessimisticLockingExample1(IBucket bucket)
        {
            // tag::PessimisticLockingExample1[]
            // create initial document
            bucket.Upsert("myshield", new { defenseLevel = 1, name = "Mirror Shield" });

            // document is retrieved and locked by A
            var shieldAResult = bucket.GetAndLock<dynamic>("myshield",TimeSpan.FromMilliseconds(30000));
            var shieldA = shieldAResult.Value;
            // B attempts to get and lock it as well
            var shieldBResult = bucket.GetAndLock<dynamic>("myshield", TimeSpan.FromMilliseconds(30000));
            if (!shieldBResult.Success)
            {
                Console.WriteLine("B couldn't establish a lock, trying a plain Get");
                shieldBResult = bucket.Get<dynamic>("myshield");
            }

            // B tries to make a change, despite not having a lock
            Console.WriteLine("'B' is updating the document");
            var shieldB = shieldBResult.Value;
            shieldB.defenseLevel = 3;
            IOperationResult bResult = bucket.Replace("myshield", shieldB);
            if (!bResult.Success)
            {
                Console.WriteLine($"B was unable to make a change: {bResult.Message}");
                Console.WriteLine();
            }

            // A can make the change, but MUST use the CAS value
            shieldA.defenseLevel = 2;
            IOperationResult aResult = bucket.Replace("myshield", shieldA);
            if (!aResult.Success)
            {
                Console.WriteLine($"A tried to make a change, but forgot to use a CAS value: {aResult.Message}");
                Console.WriteLine();
                Console.WriteLine("Trying again with CAS this time");
                aResult = bucket.Replace("myshield", shieldA, shieldAResult.Cas);
                if(aResult.Success)
                    Console.WriteLine("Success!");
            }

            // now, the document is unlocked
            // so B can try again
            bResult = bucket.Replace("myshield", shieldB);
            if (bResult.Success)
                Console.WriteLine($"B was able to make a change.");
            // end::PessimisticLockingExample1[]
        }

        private static void PessimisticLockingExample2(IBucket bucket)
        {
            // tag::PessimisticLockingExample2[]
            bucket.Upsert("mymagic", new { magicLevel = 1, name = "Fire Magic" });

            // alternatively, if A never gets around to releasing the lock
            // then the lock will automatically be released after a certain time
            var magicAResult = bucket.GetAndLock<dynamic>("mymagic", TimeSpan.FromMilliseconds(5000));
            if(magicAResult.Success)
                Console.WriteLine("Got a lock on 'mymagic'");
            // try to get a new lock every second
            for (var i = 0; i < 10; i++)
            {
                var magicBResult = bucket.GetAndLock<dynamic>("mymagic", TimeSpan.FromMilliseconds(5000));
                if (magicBResult.Success)
                {
                    Console.WriteLine("Got a new lock on 'mymagic'!");
                    bucket.Unlock("mymagic", magicBResult.Cas); // unlock it right away
                    break;
                } else {
                    Console.WriteLine("'mymagic' document is still locked.");
                }

                Thread.Sleep(1000);
            }
            // end::PessimisticLockingExample2[]
        }

        private static void OptimisticLockingExample(IBucket bucket)
        {
            // tag::OptimisticLockingExample[]
            // create initial document
            bucket.Upsert("myweapon", new {offenseLevel = 1, name = "Excalibur"});

            // document is retrieved by two different applications (A and B)
            var weaponAResult = bucket.Get<dynamic>("myweapon");
            var weaponA = weaponAResult.Value;
            var weaponBResult = bucket.Get<dynamic>("myweapon");
            var weaponB = weaponBResult.Value;
            // at this point, CAS values should be the same
            if (weaponAResult.Cas == weaponBResult.Cas)
                Console.WriteLine("CAS values are currently the same!");

            // A makes a change
            Console.WriteLine("'A' is updating the document");
            weaponA.offenseLevel = 2;
            IOperationResult aResult = bucket.Replace("myweapon", weaponA, weaponAResult.Cas);
            if (aResult.Success)
                Console.WriteLine($"Change by 'A' was successful. New CAS value: {aResult.Cas}");

            // B tries to make a change too
            Console.WriteLine($"'B' is (attempting to) update the same document using old CAS value: {weaponBResult.Cas}");
            weaponB.offenseLevel = 3;
            IOperationResult bResult = bucket.Replace("myweapon", weaponB, weaponBResult.Cas);
            if (!bResult.Success)
                Console.WriteLine($"Change by B failed: {bResult.Exception.Message}");
            // end::OptimisticLockingExample[]
        }
    }
}

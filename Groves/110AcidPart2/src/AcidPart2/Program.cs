using System;
using System.Collections.Generic;
using Couchbase;
using Couchbase.Configuration.Client;
using Couchbase.Core;

namespace AcidPart2
{
    class Program
    {
        static void Main(string[] args)
        {
            var cluster = new Cluster(new ClientConfiguration
            {
                Servers = new List<Uri> { new Uri("http://localhost:8091") }
            });
            cluster.Authenticate("matt", "password");
            var bucket = cluster.OpenBucket("barns");

            var errorHappensDuringPending = false;
            var errorHappensAfterCommitted = false;

            var random = new Random();
            var barn1Key = "barn::" + Guid.NewGuid();
            var barn2Key = "barn::" + Guid.NewGuid();
            bucket.Insert(barn1Key, new Barn { Name = Faker.NameFaker.LastName() + " Barn", Chickens = random.Next(1, 30) });
            bucket.Insert(barn2Key, new Barn { Name = Faker.NameFaker.LastName() + " Barn", Chickens = random.Next(1, 30) });
            var barn1 = bucket.Get<Barn>(barn1Key, TimeSpan.FromSeconds(30));
            var barn2 = bucket.Get<Barn>(barn2Key, TimeSpan.FromSeconds(30));

            Console.WriteLine($"{barn1.Value.Name} has {barn1.Value.Chickens} chickens.");
            Console.WriteLine($"{barn2.Value.Name} has {barn2.Value.Chickens} chickens.");
            var amountToTransfer = 1;
            Console.WriteLine($"Transferring {amountToTransfer} chickens from first barn to second.");
            Console.ReadLine();

            // **** start transaction

            // create transation document
            var transactionDocumentNumber = bucket.Increment("transaction::counter");
            var transactionDocumentKey = $"transaction::{transactionDocumentNumber.Value}";

            var transaction = bucket.Upsert(new Document<CouchbaseTransaction>
            {
                Id = transactionDocumentKey,
                Content = new CouchbaseTransaction
                {
                    SourceId = barn1.Id,
                    Source = barn1.Value,
                    DestinationId = barn2.Id,
                    Destination = barn2.Value,
                    Amount = amountToTransfer,
                    State = TransactionStates.Initial
                }
            });

            try
            {
                // #1 switch to pending
                bucket.UpdateWithCas<CouchbaseTransaction>(transaction.Id, x => x.State = TransactionStates.Pending);
                Console.WriteLine($"Switch transaction to {TransactionStates.Pending}");
                Console.ReadLine();

                // #2 apply transaction to both documents
                // #2a update source MINUS amount
                bucket.UpdateWithCas<Barn>(barn1Key, x => { x.Chickens -= amountToTransfer; x.Transactions.Add(transaction.Id); });
                Console.WriteLine($"Subtract {amountToTransfer} from {barn1.Value.Name} (and mark transaction {transaction.Id})");
                Console.ReadLine();

                if (errorHappensDuringPending)
                    throw new Exception("Something went wrong while transaction was pending.");

                // #2b update destination PLUS amount
                bucket.UpdateWithCas<Barn>(barn2Key, x => { x.Chickens += amountToTransfer; x.Transactions.Add(transaction.Id); });
                Console.WriteLine($"Add {amountToTransfer} to {barn2.Value.Name} (and mark transaction {transaction.Id})");
                Console.ReadLine();

                // #3 switch transaction to committed
                bucket.UpdateWithCas<CouchbaseTransaction>(transaction.Id, x => x.State = TransactionStates.Committed );
                Console.WriteLine($"Switch transaction to {TransactionStates.Committed}");
                Console.ReadLine();

                if(errorHappensAfterCommitted)
                    throw new Exception("Something went wrong after transaction was committed.");

                // #4 remove transactions from document
                bucket.UpdateWithCas<Barn>(barn1Key, x => { x.Transactions.Remove(transaction.Id); });
                bucket.UpdateWithCas<Barn>(barn2Key, x => { x.Transactions.Remove(transaction.Id); });
                Console.WriteLine("Remove transactions from barns.");
                Console.ReadLine();

                // #5 switch transaction to done
                bucket.UpdateWithCas<CouchbaseTransaction>(transaction.Id, x => x.State = TransactionStates.Done);
                Console.WriteLine($"Switch transaction to {TransactionStates.Done}");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ROLLING BACK: {ex.Message}");
                // rollback transaction
                var t = bucket.Get<CouchbaseTransaction>(transaction.Id);
                switch (t.Value.State)
                {
                    case TransactionStates.Committed:
                        // create new transaction and swap the targets
                        // TODO: recursive call
                    break;
                    case TransactionStates.Pending:
                        // #1 -> switch to 'cancelling' state
                        bucket.UpdateWithCas<CouchbaseTransaction>(transaction.Id, x => x.State = TransactionStates.Cancelling, t.Cas);
                        Console.WriteLine($"Switch transaction to {TransactionStates.Cancelling}");
                        Console.ReadLine();

                        // #2 -> revent changes if they were applied
                        bucket.UpdateWithCas<Barn>(barn1Key, x =>
                        {
                            if (x.Transactions.Contains(transaction.Id))
                            {
                                x.Chickens += amountToTransfer;
                                x.Transactions.Remove(transaction.Id);
                            }
                        });
                        Console.WriteLine($"Add {amountToTransfer} to {barn1.Value.Name} (and remove transaction {transaction.Id})");
                        Console.ReadLine();

                        bucket.UpdateWithCas<Barn>(barn2Key, x =>
                        {
                            if (x.Transactions.Contains(transaction.Id))
                            {
                                x.Chickens -= amountToTransfer;
                                x.Transactions.Remove(transaction.Id);
                            }
                        });
                        Console.WriteLine($"Remove {amountToTransfer} from {barn2.Value.Name} (and remove transaction {transaction.Id})");
                        Console.ReadLine();

                        // #3 -> switch to cancelled state
                        bucket.UpdateWithCas<CouchbaseTransaction>(transaction.Id, x => x.State = TransactionStates.Cancelled);
                        Console.WriteLine($"Switch transaction to {TransactionStates.Cancelled}");
                        Console.ReadLine();
                    break;
                }

                throw;
            }

            // **** end transaction

            bucket.Unlock(barn1Key, barn1.Cas);
            bucket.Unlock(barn2Key, barn2.Cas);
            cluster.Dispose();
        }

        public abstract class Transactionable
        {
            public List<string> Transactions { get; set; } = new List<string>();
        }


        public class Barn : Transactionable
        {
            public int Chickens { get; set; }
            public string Name { get; set; }
        }

        public enum TransactionStates
        {
            Initial = 0,
            Pending = 1,
            Committed = 2,
            Done = 3,
            Cancelling,
            Cancelled
        }

        public class CouchbaseTransaction
        {
            public string SourceId { get; set; }
            public Barn Source { get; set; }
            public string DestinationId { get; set; }
            public Barn Destination { get; set; }
            public int Amount { get; set; }
            public TransactionStates State { get; set; }
        }
    }

    public static class CouchbaseHelpers
    {
        public static void UpdateWithCas<T>(this IBucket @this, string documentId, Action<T> act, ulong? cas = null)
        {
            var document = @this.Get<T>(documentId);
            var content = document.Value;
            act(content);
            @this.Replace(new Document<T>
            {
                Cas = cas ?? document.Cas,
                Id = document.Id,
                Content = content
            });
        }
    }
}

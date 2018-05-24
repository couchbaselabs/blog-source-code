using System;
using Couchbase;
using Couchbase.Core;

namespace AcidPart2
{
    public class TransactionHelper
    {
        readonly IBucket _bucket;
        readonly bool _simulateErrorDuringPending;
        readonly bool _simulateErrorsAfterCommitted;

        public TransactionHelper(IBucket bucket, bool simulateErrorDuringPending = false, bool simulateErrorsAfterCommitted = false)
        {
            _bucket = bucket;
            _simulateErrorDuringPending = simulateErrorDuringPending;
            _simulateErrorsAfterCommitted = simulateErrorsAfterCommitted;
        }

        public void Perform(IOperationResult<Barn> source, IOperationResult<Barn> destination, int amountToTransfer)
        {
            // this is how you create incremental document keys in Couchbase
            // you could use GUID or some other way to generate keys, it's not critical which you use
            var transactionDocumentNumber = _bucket.Increment("transaction::counter");
            var transactionDocumentKey = $"transaction::{transactionDocumentNumber.Value}";

            // create transation document
            // this is the record of a transaction, contains details of the transaction
            // used to keep track of transaction, and can also help debug/diagnose
            var transaction = _bucket.Upsert(new Document<TransactionRecord>
            {
                Id = transactionDocumentKey,
                Content = new TransactionRecord
                {
                    SourceId = source.Id,
                    Source = source.Value,
                    DestinationId = destination.Id,
                    Destination = destination.Value,
                    Amount = amountToTransfer,
                    State = TransactionStates.Initial
                }
            });
            Console.WriteLine($"Created transaction record: {transaction.Id}");

            try
            {
                // #1 switch the transaction to pending - the transaction is now in progress
                // and a rollback attempt will see "pending" status and know how to proceed
                UpdateWithCas<TransactionRecord>(transaction.Id, x => x.State = TransactionStates.Pending);
                Console.WriteLine($"Switch transaction to {TransactionStates.Pending}");
                Console.ReadLine();

                // #2 apply the change to both documents - this method is only able to transfer an amount of chickens
                //      but it could be made more generic for your use case(s)
                // #2a update source MINUS amount
                UpdateWithCas<Barn>(source.Id, x => { x.Chickens -= amountToTransfer; x.Transactions.Add(transaction.Id); });
                Console.WriteLine($"Subtract {amountToTransfer} from {source.Value.Name} (and mark transaction {transaction.Id})");
                Console.ReadLine();

                // simulate an error that occurs smack in the middle of the transaction
                // triggering a rollback in the "catch"
                if (_simulateErrorDuringPending)
                    throw new Exception("Something went wrong while transaction was pending.");

                // #2b update destination PLUS amount
                UpdateWithCas<Barn>(destination.Id, x => { x.Chickens += amountToTransfer; x.Transactions.Add(transaction.Id); });
                Console.WriteLine($"Add {amountToTransfer} to {destination.Value.Name} (and mark transaction {transaction.Id})");
                Console.ReadLine();

                // #3 switch transaction to committed
                // the transaction is now complete, and needs to be cleaned up
                UpdateWithCas<TransactionRecord>(transaction.Id, x => x.State = TransactionStates.Committed);
                Console.WriteLine($"Switch transaction to {TransactionStates.Committed}");
                Console.ReadLine();

                // simulate an error during the committed/cleanup phase
                // triggering a rollback in the "catch"
                if (_simulateErrorsAfterCommitted)
                    throw new Exception("Something went wrong after transaction was committed.");

                // #4 remove transactions from document
                // each document was marked as being part of a transaction
                // that needs to be cleaned up
                UpdateWithCas<Barn>(source.Id, x => { x.Transactions.Remove(transaction.Id); });
                UpdateWithCas<Barn>(destination.Id, x => { x.Transactions.Remove(transaction.Id); });
                Console.WriteLine("Remove transactions from barns.");
                Console.ReadLine();

                // #5 switch transaction to done
                // cleanup in complete, so switch transaction to 'done'
                // transactions in 'done' status could hypothetically be deleted on a regular basis
                // but you may want to keep them around for a while just in case
                UpdateWithCas<TransactionRecord>(transaction.Id, x => x.State = TransactionStates.Done);
                Console.WriteLine($"Switch transaction to {TransactionStates.Done}");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ROLLING BACK: {ex.Message}");
                Rollback(source, destination, amountToTransfer, transaction, ex);

                throw;
            }
        }

        private void Rollback(IOperationResult<Barn> source, IOperationResult<Barn> destination, int amountToTransfer, IDocumentResult<TransactionRecord> transaction, Exception ex)
        {
            var transactionRecord = _bucket.Get<TransactionRecord>(transaction.Id);
            switch (transactionRecord.Value.State)
            {
                case TransactionStates.Committed:
                    // create new transaction and swap the targets
                    // since the changes were already made to the documents, the amounts
                    // must be swapped back
                    Perform(destination, source, 1);
                    break;
                case TransactionStates.Pending:
                    // #1 -> switch to 'cancelling' state
                    UpdateWithCas<TransactionRecord>(transaction.Id, x => x.State = TransactionStates.Cancelling, transactionRecord.Cas);
                    Console.WriteLine($"Switch transaction to {TransactionStates.Cancelling}");
                    Console.ReadLine();

                    // #2 -> revent changes if they were applied
                    UpdateWithCas<Barn>(source.Id, x =>
                    {
                        if (x.Transactions.Contains(transaction.Id))
                        {
                            x.Chickens += amountToTransfer;
                            x.Transactions.Remove(transaction.Id);
                        }
                    });
                    Console.WriteLine($"Add {amountToTransfer} to {source.Value.Name} (and remove transaction {transaction.Id})");
                    Console.ReadLine();

                    UpdateWithCas<Barn>(destination.Id, x =>
                    {
                        if (x.Transactions.Contains(transaction.Id))
                        {
                            x.Chickens -= amountToTransfer;
                            x.Transactions.Remove(transaction.Id);
                        }
                    });
                    Console.WriteLine($"Remove {amountToTransfer} from {destination.Value.Name} (and remove transaction {transaction.Id})");
                    Console.ReadLine();

                    // #3 -> switch to cancelled state
                    UpdateWithCas<TransactionRecord>(transaction.Id, x => x.State = TransactionStates.Cancelled);
                    Console.WriteLine($"Switch transaction to {TransactionStates.Cancelled}");
                    Console.ReadLine();
                    break;
            }
        }

        // this is a helper method to get a document and make a change using
        // optimistic locking
        // a CAS value can optionally be passed in
        private void UpdateWithCas<T>(string documentId, Action<T> act, ulong? cas = null)
        {
            var document = _bucket.Get<T>(documentId);
            var content = document.Value;
            act(content);
            _bucket.Replace(new Document<T>
            {
                Cas = cas ?? document.Cas,
                Id = document.Id,
                Content = content
            });
            // NOTE: could put retr(ies) here
        }
    }
}
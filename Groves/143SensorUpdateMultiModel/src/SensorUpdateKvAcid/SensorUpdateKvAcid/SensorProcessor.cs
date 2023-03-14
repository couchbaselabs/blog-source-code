using Couchbase;
using Couchbase.KeyValue;
using Couchbase.Transactions;
using Couchbase.Transactions.Config;

namespace SensorUpdateKvAcid;

public class SensorProcessor
{
    private readonly ICluster _cluster;
    private readonly ICouchbaseCollection _collection;

    public SensorProcessor(ICluster cluster, ICouchbaseCollection collection)
    {
        _cluster = cluster;
        _collection = collection;
    }

    public async Task ProcessIncomingSensorReading(string sensorId, NauticalSpeed newSensorReading)
    {
        var transaction = Transactions.Create(_cluster, TransactionConfigBuilder.Create()
            .DurabilityLevel(DurabilityLevel.None)); // set to 'none' because I'm using a single-node dev cluster

        // for more details see: https://docs.couchbase.com/dotnet-sdk/current/howtos/distributed-acid-transactions-from-the-sdk.html
        await transaction.RunAsync(async (context) =>
        {
            // get existing sensor reading
            var currentDoc = await context.GetAsync(_collection, sensorId);
            var currentReading = currentDoc.ContentAs<NauticalSpeed>();

            // check timestamps
            if (newSensorReading.TimeStamp > currentReading.TimeStamp)
            {
                // incoming reading is newer, update the record
                Console.WriteLine("Incoming sensor reading is newer. Updating.");
                await context.ReplaceAsync(currentDoc, newSensorReading);
            }
            else
            {
                Console.WriteLine("Incoming sensor reading is not new. Ignoring.");
                // incoming reading is not newer, so do nothing
                // (or possibly update a log, or whatever else you want to do)
            }
        });
    }
}
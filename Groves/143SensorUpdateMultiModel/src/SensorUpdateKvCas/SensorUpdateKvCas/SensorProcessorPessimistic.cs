using Couchbase.KeyValue;

namespace SensorUpdateKvCas;

public class SensorProcessorPessimistic
{
    private readonly ICouchbaseCollection _collection;

    public SensorProcessorPessimistic(ICouchbaseCollection collection)
    {
        _collection = collection;
    }

    public async Task ProcessIncomingSensorReading(string sensorId, NauticalSpeed newSensorReading)
    {
        // get existing sensor reading
        var maxLockTime = TimeSpan.FromSeconds(30);
        var currentDoc = await _collection.GetAndLockAsync(sensorId, maxLockTime);
        var currentDocCas = currentDoc.Cas;
        var currentReading = currentDoc.ContentAs<NauticalSpeed>();

        // check timestamps
        if (newSensorReading.TimeStamp > currentReading.TimeStamp)
        {
            // incoming reading is newer, update the record
            Console.WriteLine("Incoming sensor reading is newer. Updating.");
            await _collection.ReplaceAsync(sensorId, newSensorReading, options => options.Cas(currentDocCas));
            return;
        }
        else
        {
            await _collection.UnlockAsync(sensorId, currentDocCas);
            Console.WriteLine("Incoming sensor reading is not new. Ignoring.");
            // incoming reading is not newer, so do nothing
            // (or possibly update a log, or whatever else you want to do)
        }
    }
}
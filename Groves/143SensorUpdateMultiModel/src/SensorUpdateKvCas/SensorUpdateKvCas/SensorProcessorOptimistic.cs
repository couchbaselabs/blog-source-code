using Couchbase.Core.Exceptions;
using Couchbase.KeyValue;

namespace SensorUpdateKvCas;

public class SensorProcessorOptimistic
{
    private readonly ICouchbaseCollection _collection;

    public SensorProcessorOptimistic(ICouchbaseCollection collection)
    {
        _collection = collection;
    }

    public async Task ProcessIncomingSensorReading(string sensorId, NauticalSpeed newSensorReading)
    {
            // get existing sensor reading
            var currentDoc = await _collection.GetAsync(sensorId);
            var currentDocCas = currentDoc.Cas;
            var currentReading = currentDoc.ContentAs<NauticalSpeed>();

            // check timestamps
            if (newSensorReading.TimeStamp > currentReading.TimeStamp)
            {
                // incoming reading is newer, update the record
                Console.WriteLine("Incoming sensor reading is newer. Updating.");
                var retries = 3;
                while (retries > 0)
                {
                    try
                    {
                        await _collection.ReplaceAsync(sensorId, newSensorReading, options => options.Cas(currentDocCas));
                        return;
                    }
                    catch (CasMismatchException)
                    {
                        Console.WriteLine($"CAS mismatch. Retries remaining: {retries}");
                        retries--;
                    }
                }
                Console.WriteLine("Retry max exceeded. Sensor reading was not updated.");
            }
            else
            {
                Console.WriteLine("Incoming sensor reading is not new. Ignoring.");
                // incoming reading is not newer, so do nothing
                // (or possibly update a log, or whatever else you want to do)
            }
    }
}
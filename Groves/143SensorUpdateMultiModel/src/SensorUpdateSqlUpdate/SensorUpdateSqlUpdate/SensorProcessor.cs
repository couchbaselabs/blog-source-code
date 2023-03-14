using Couchbase;
using Couchbase.Core.Exceptions;

namespace SensorUpdateSqlUpdate;

public class SensorProcessor
{
    private readonly ICluster _cluster;

    public SensorProcessor(ICluster cluster)
    {
        _cluster = cluster;
    }

    public async Task ProcessIncomingSensorReading(string sensorId, NauticalSpeed sensorReading)
    {
        var retries = 3;
        while (retries > 0)
        {
            try
            {
                await _cluster.QueryAsync<dynamic>(@"UPDATE sensordata s
                    USE KEYS $sensorId
                    SET s.speed = $newSpeed, s.unit = $newUnit, s.timeStamp = $newTimeStamp
                    WHERE DATE_DIFF_STR($newTimeStamp, s.timeStamp, 'millisecond') > 0", options =>
                {
                    options.Parameter("sensorId", sensorId);
                    options.Parameter("newSpeed", sensorReading.Speed);
                    options.Parameter("newUnit", sensorReading.Unit);
                    options.Parameter("newTimeStamp", sensorReading.TimeStamp);
                });
                return;
            }
            catch (CasMismatchException)
            {
                Console.WriteLine($"UPDATE CAS mismatch, tries remaining: {retries}");
                retries--;
            }
        }
        Console.WriteLine("Max retries exceeded, sensor not updated");
    }
}
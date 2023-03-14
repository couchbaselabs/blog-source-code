using Couchbase;
using SensorUpdateKvCas;

// connect to Couchbase
var cluster = await Cluster.ConnectAsync("couchbase://localhost", options =>
{
    options.UserName = "Administrator";
    options.Password = "password";
});
var bucket = await cluster.BucketAsync("sensordata");
var collection = await bucket.DefaultCollectionAsync();

// create some initial data - Nautical Speed for two different cruise ships
var cdiSensor = new NauticalSpeed
{
    Speed = 30,
    Unit = "knots",
    TimeStamp = new DateTime(2023, 3, 10, 10, 30, 00)
};
var cmeSensor = new NauticalSpeed
{
    Speed = 16,
    Unit = "knots",
    TimeStamp = new DateTime(2023, 3, 10, 10, 30, 01)
};
await collection.InsertAsync("C-DI_Nautical_Speed", cdiSensor);
await collection.InsertAsync("C-ME_Nautical_Speed", cmeSensor);

Console.WriteLine("Sample data created. Press ENTER to continue...");
Console.ReadLine();

// process incoming sensor readings
var processorOpt = new SensorProcessorOptimistic(collection);
//var processorPess = new SensorProcessorPessimistic(collection);

// an update where the incoming sensor reading DOES have a later timestamp
var newCdiSensor = new NauticalSpeed
{
    Speed = 28,
    Unit = "knots",
    TimeStamp = cdiSensor.TimeStamp.Add(TimeSpan.FromSeconds(30))
};
await processorOpt.ProcessIncomingSensorReading("C-DI_Nautical_Speed", newCdiSensor);
//await processorPess.ProcessIncomingSensorReading("C-DI_Nautical_Speed", newCdiSensor);

Console.WriteLine("First incoming sensor processed (optimistic). Press ENTER to continue...");
Console.ReadLine();

// an update where the latest received data DOES NOT have a later timestamp
var newCmeSensor = new NauticalSpeed
{
    Speed = 14,
    Unit = "knots",
    TimeStamp = cmeSensor.TimeStamp.Subtract(TimeSpan.FromSeconds(30))
};
await processorOpt.ProcessIncomingSensorReading("C-ME_Nautical_Speed", newCmeSensor);
//await processorPess.ProcessIncomingSensorReading("C-ME_Nautical_Speed", newCmeSensor);

Console.WriteLine("Second incoming sensor processed (pessimistic). Press ENTER to continue...");
Console.ReadLine();

// cleanup/remove data
await collection.RemoveAsync("C-DI_Nautical_Speed");
await collection.RemoveAsync("C-ME_Nautical_Speed");

Console.WriteLine("Data removed. Press ENTER to continue...");
Console.ReadLine();

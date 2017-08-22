using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.Configuration.Client;

namespace GenerateData
{
    /*
     * key jason::1
     * {
  "dealerId": "1",
  "id": "1",
  "type": "jasonTestEmployee"
}

        key jason::2
        {
  "dealerId": "1",
  "id": "2",
  "type": "jasonTestEmployee"
}

        key jason::3
        {
  "dealerId": "2",
  "id": "3",
  "type": "jasonTestEmployee"
}

        key jason::4
        {
  "dealerId": "2",
  "id": "4",
  "type": "jasonTestEmployee"
}
     */

    class Program
    {
        static void Main(string[] args)
        {
            // generate patient documents
            // each doctor has a random number of patients
            // each patient has a "doctorId" value of their doctor
            // there might be doctor documents in a real app
            // but not necessary for this tutorial

            var rand = new Random();
            var cluster = new Cluster(new ClientConfiguration
            {
                Servers = new List<Uri> {new Uri("http://localhost:8091")}
            });
            var bucket = cluster.OpenBucket("patients");

            // let's assume 100 doctors
            for (int doctorId = 0; doctorId < 100; doctorId++)
            {
                // each doctor has somewhere between 1 and 12 employees
                var numPatients = rand.Next(1, 13);

                // insert patient document, each document has
                //  doctorId - of their doctor
                //  patientName - their name
                //  patientDob - their DOB
                // name and dob aren't strictly necessary for the demo either
                for (var i = 0; i < numPatients; i++)
                {
                    var patientId = Faker.StringFaker.AlphaNumeric(8); // Guid.NewGuid().ToString();
                    bucket.Insert(patientId, new
                    {
                        doctorId = doctorId,
                        patientName = Faker.NameFaker.Name(),
                        patientDob = Faker.DateTimeFaker.BirthDay(18)
                    });
                }
                Console.WriteLine($"doctor [{doctorId}] - {numPatients} patient(s)");
            }

            cluster.Dispose();
        }
    }
}

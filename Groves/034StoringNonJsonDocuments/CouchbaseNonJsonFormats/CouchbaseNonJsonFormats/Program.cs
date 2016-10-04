using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using Couchbase;
using Couchbase.Configuration.Client;

namespace CouchbaseNonJsonFormats
{
    class Program
    {
        static void Main(string[] args)
        {
            ClientConfiguration clientConfig = new ClientConfiguration();
            clientConfig.Servers = new List<Uri> {new Uri("couchbase://localhost")};
            ClusterHelper.Initialize(clientConfig);
            var bucket = ClusterHelper.GetBucket("default");

            var guid = Guid.NewGuid().ToString();
            var objectToSerialize = new MyType {Foo = "bar"};

            // default serialization is json
            bucket.Insert<MyType>("JSON_" + guid, objectToSerialize);

            // xml serialization
            var xml = new XmlSerializer(objectToSerialize.GetType());
            using (var textWriter = new StringWriter())
            {
                xml.Serialize(textWriter, objectToSerialize);
                bucket.Insert<string>("XML_" + guid, textWriter.ToString());
            }

            // byte serialization
            var formatter = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                formatter.Serialize(ms, objectToSerialize);
                bucket.Insert<byte[]>("byte_" + guid, ms.ToArray());
            }

            ClusterHelper.Close();
        }
    }

    [Serializable]  // this is for BinaryFormatter
    public class MyType
    {
        public string Foo { get; set; }
    }
}

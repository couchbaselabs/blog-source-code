using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
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

            // default serialization is json
            Console.WriteLine("Inserting JSON document");
            // tag::JSON[]
            // insert document
            bucket.Insert<MyType>("JSON_" + guid, new MyType { Foo = "BarJSON"});
            // get the document back out and display it
            var jsonBackOut = bucket.Get<MyType>("JSON_" + guid).Value;
            Console.WriteLine($"JSON document: {jsonBackOut.Foo}");
            // end::JSON[]

            Console.WriteLine();

            // xml serialization
            Console.WriteLine("Inserting XML value");
            // tag::XML[]
            var xmlo = new MyType {Foo = "BarXML"};
            var xml = new XmlSerializer(xmlo.GetType());
            using (var textWriter = new StringWriter())
            {
                xml.Serialize(textWriter, xmlo);
                bucket.Insert<string>("XML_" + guid, textWriter.ToString());
            }
            // get the XML back out, deserialize it, display object
            var xmlBackOut = bucket.Get<string>("XML_" + guid).Value;
            using (var reader = new StringReader(xmlBackOut))
            {
                var xmlObject = (MyType)xml.Deserialize(reader);
                Console.WriteLine($"XML: {xmlObject.Foo}");
            }
            // end::XML[]

            Console.WriteLine();

            // byte serialization
            Console.WriteLine("Inserting .NET serialized value");
            // tag::bytes[]
            var formatter = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                formatter.Serialize(ms, new MyType { Foo = "BarDotNET"});
                bucket.Insert<byte[]>("byte_" + guid, ms.ToArray());
            }
            // get the bytes back out, deserialize them, display object
            var bytesBackOut = bucket.Get<byte[]>("byte_" + guid).Value;
            using (var stream = new MemoryStream(bytesBackOut))
            {
                var bytesObject = (MyType)formatter.Deserialize(stream);
                Console.WriteLine($".NET: {bytesObject.Foo}");
            }
            // end::bytes[]

            ClusterHelper.Close();
        }
    }

    [Serializable]  // this is for BinaryFormatter
    public class MyType
    {
        public string Foo { get; set; }
    }
}

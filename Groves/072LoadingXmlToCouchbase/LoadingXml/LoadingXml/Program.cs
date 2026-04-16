using System;
using System.Xml;
using Couchbase;
using Couchbase.KeyValue;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace LoadingXml
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // sample XML, parsed into an XmlDocument
            // this might come from an XML file, another database, a REST API, etc
            // but for this example, it's just a hardcoded string
            // tag::xml[]
            var xml = @"
                <Invoice>
                    <Timestamp>4/16/2026 02:23</Timestamp>
                    <CustNumber>12345</CustNumber>
                    <AcctNumber>54321</AcctNumber>
                </Invoice>";
            // end::xml[]
            // tag::xmldocument[]
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            // end::xmldocument[]

            // convert XML into JSON using Newtonsoft Json.net
            // tag::jsonconvert[]
            var json = JsonConvert.SerializeXmlNode(doc, Formatting.None, true);
            // end::jsonconvert[]

            // this is just an example of what the Json would look like if I DIDN'T omit root node
            // {"Invoice":{"Timestamp":"4/16/2026 02:23","CustNumber":"12345","AcctNumber":"54321"}}

            // connect to couchbase cluster
            var cluster = await Cluster.ConnectAsync(
                "couchbases://cb.ojzftkgudoak8tkc.cloud.couchbase.com",
                "xmlconvert",
                "5M1+cjb$LAhP"
            );
            var bucket = await cluster.BucketAsync("loadxml");
            var collection = bucket.DefaultCollection();

            // insert directly (literal translation)
            // tag::insertobject[]
            object transactObject1 = JsonConvert.DeserializeObject(json);
            await collection.InsertAsync(Guid.NewGuid().ToString(), transactObject1);
            // end::insertobject[]

            // insert via class (type information, naming conventions applied)
            // tag::insertobject2[]
            Invoice transactObject2 = JsonConvert.DeserializeObject<Invoice>(json);
            await collection.InsertAsync(Guid.NewGuid().ToString(), transactObject2);
            // end::insertobject2[]

            await cluster.DisposeAsync();
        }
    }

    // tag::invoiceclass[]
    public class Invoice
    {
        public DateTime Timestamp { get; set; }
        public string CustNumber { get; set; }
        public int AcctNumber { get; set; }
    }
    // end::invoiceclass[]
}
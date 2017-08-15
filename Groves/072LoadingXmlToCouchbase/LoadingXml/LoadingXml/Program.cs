using System;
using System.Collections.Generic;
using System.Xml;
using Couchbase;
using Couchbase.Configuration.Client;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace LoadingXml
{
    // http://www.newtonsoft.com/json/help/html/ConvertingJSONandXML.htm
    // http://www.newtonsoft.com/json/help/html/Introduction.htm

    class Program
    {
        static void Main(string[] args)
        {
            // sample XML, parsed into an XmlDocument
            // this might come from an XML file, another database, a REST API, etc
            // but for this example, it's just a hardcoded string
            // tag::xml[]
            var xml = @"
<Invoice>
    <Timestamp>1/1/2017 00:01</Timestamp>
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
            // {"Invoice":{"Timestamp":"1/1/2017 00:01","CustNumber":"12345","AcctNumber":"54321"}}

            // connect to couchbase cluster
            ClusterHelper.Initialize(new ClientConfiguration
            {
                Servers = new List<Uri> {new Uri("http://localhost:8091")}
            });
            var bucket = ClusterHelper.GetBucket("loadxml", "password");

            // insert directly (literal translation)
            // tag::insertobject[]
            object transactObject1 = JsonConvert.DeserializeObject(json);
            bucket.Insert(Guid.NewGuid().ToString(), transactObject1);
            // end::insertobject[]

            // insert via class (type information, naming conventions applied)
            // tag::insertobject2[]
            Invoice transactObject2 = JsonConvert.DeserializeObject<Invoice>(json);
            bucket.Insert(Guid.NewGuid().ToString(), transactObject2);
            // end::insertobject2[]
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

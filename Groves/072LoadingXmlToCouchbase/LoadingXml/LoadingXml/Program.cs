using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using Couchbase;

class Program
{
    static async Task Main(string[] args)
    {
        string xml = @"<user id='1'>
                        <name>Alice</name>
                        <email>alice@example.com</email>
                        <roles>
                            <role>admin</role>
                            <role>editor</role>
                        </roles>
                       </user>";

        Console.WriteLine("=== ORIGINAL XML ===");
        Console.WriteLine(xml);

        // =========================
        // OPTION 1: Newtonsoft Json.NET
        // =========================
        Console.WriteLine("\n=== NEWTONSOFT JSON.NET ===");

        // tag::newtonsoftjsonconvert[]
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xml);

        string jsonNewtonsoft =
            JsonConvert.SerializeXmlNode(xmlDoc, Newtonsoft.Json.Formatting.Indented);
        // end::newtonsoftjsonconvert[]

        Console.WriteLine(jsonNewtonsoft);
        Console.ReadLine();

        // =========================
        // OPTION 2: System.Text.Json (manual)
        // =========================
        Console.WriteLine("\n=== SYSTEM.TEXT.JSON ===");

        // tag::systemjsonconvert[]
        var xdoc = XDocument.Parse(xml);
        var dict = XmlToDictionary(xdoc.Root!);

        string jsonSystemText = System.Text.Json.JsonSerializer.Serialize(
            dict,
            new JsonSerializerOptions { WriteIndented = true });
        // end::systemjsonconvert[]

        Console.WriteLine(jsonSystemText);
        Console.ReadLine();

        // =========================
        // COUCHBASE DEMO
        // =========================
        // tag::couchbase[]
        var cluster = await Cluster.ConnectAsync(
            "couchbases://cb.<connectionString>.cloud.couchbase.com",
            new ClusterOptions
            {
                UserName = "xmlconvert",
                Password = "password"
            });

        var bucket = await cluster.BucketAsync("loadxml");
        var collection = await bucket.DefaultCollectionAsync();

        var jsonObj = JObject.Parse(jsonNewtonsoft);
        var documentId = $"user::{jsonObj["user"]!["@id"]}";

        await collection.UpsertAsync(documentId, jsonObj);
        // end::couchbase[]

        Console.WriteLine($"Stored document: {documentId}");
        Console.ReadLine();

        // =========================
        // BULK DEMO (simulated)
        // =========================
        Console.WriteLine("\n=== BULK CONVERSION DEMO ===");

        string xmlA = @"<user id='2'>
                        <name>Matt</name>
                        <email>matt@example.com</email>
                        <roles>
                            <role>admin</role>
                        </roles>
                       </user>";
        string xmlB = @"<user id='3'>
                        <name>Emma</name>
                        <email>emma@example.com</email>
                        <roles>
                            <role>editor</role>
                        </roles>
                       </user>";


        // tag::bulk[]
        var xmlSamples = new List<string> { xmlA, xmlB };

        var tasks = xmlSamples.Select(async (x, i) =>
        {
            var doc = new XmlDocument();
            doc.LoadXml(x);

            var json = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.None);
            var obj = JObject.Parse(json);

            var id = $"doc::{i}";
            await collection.UpsertAsync(id, obj);

            await Task.Delay(10); // simulate async work

            Console.WriteLine($"Processed {id}");
        });

        await Task.WhenAll(tasks);
        // end::bulk[]


        Console.WriteLine("\nDone.");
         Console.ReadLine();
   }

    // =========================
    // XML -> Dictionary helper
    // =========================
    // tag::XmlToDictionary[]
    static object XmlToDictionary(XElement element)
    {
        var hasElements = element.Elements().Any();

        // leaf node
        if (!hasElements)
        {
            return element.Value;
        }

        var dict = new Dictionary<string, object>();

        // group children by name (handles arrays)
        foreach (var group in element.Elements().GroupBy(e => e.Name.LocalName))
        {
            if (group.Count() == 1)
            {
                dict[group.Key] = XmlToDictionary(group.First());
            }
            else
            {
                dict[group.Key] = group
                    .Select(XmlToDictionary)
                    .ToList();
            }
        }

        // attributes (prefix with @ to match Newtonsoft style)
        foreach (var attr in element.Attributes())
        {
            dict[$"@{attr.Name.LocalName}"] = attr.Value;
        }

        return dict;
    }
    // end::XmlToDictionary[]
}
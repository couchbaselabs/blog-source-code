
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.KeyVault;
using System.Net.Http;
using Couchbase;
using Couchbase.Configuration.Client;
using System.Collections.Generic;
using Couchbase.Core;
using System.Net;
using System.Net.Http.Formatting;

namespace CbKeyVault.Function
{
    public static class HttpKeyVaultDemo
    {
        private static HttpClient client = new HttpClient();

        // tag::uris[]
        const string Vault_Bucketname_Uri = "https://mycouchbasekeyvault.vault.azure.net/secrets/cbBucketName/1bda709e1372465a8f03b6e8c3fb6014";
        const string Vault_Clusteruri_Uri = "https://mycouchbasekeyvault.vault.azure.net/secrets/cbClusterUri/48605e696b3645a6a7c396a15d636dc2";
        const string Vault_Username_Uri = "https://mycouchbasekeyvault.vault.azure.net/secrets/cbUsername/7d73ef5fa2174e5491d4a50a42bb0800";
        const string Vault_Password_Uri = "https://mycouchbasekeyvault.vault.azure.net/secrets/cbPassword/d6f61ff7e41a4fdcbe17de0b1fe1f115";
        // end::uris[]

        // tag::lazy[]
        private static readonly Lazy<IBucket> Bucket = new Lazy<IBucket>(() =>
        {
            var uri = GetSecret(Vault_Clusteruri_Uri);
            var bucketName = GetSecret(Vault_Bucketname_Uri);
            var username = GetSecret(Vault_Username_Uri);
            var password = GetSecret(Vault_Password_Uri);
            var cluster = new Cluster(new ClientConfiguration
            {
                Servers = new List<Uri> { new Uri(uri) }
            });
            cluster.Authenticate(username, password);
            return cluster.OpenBucket(bucketName);
        });
        // end::lazy[]

        // tag::GetSecret[]
        private static string GetSecret(string url)
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var kvClient = new KeyVaultClient(
                new KeyVaultClient.AuthenticationCallback(
                    azureServiceTokenProvider.KeyVaultTokenCallback), client);
            var secret = kvClient.GetSecretAsync(url).Result.Value;
            return secret;
        }
        // end::GetSecret[]

        [FunctionName("HttpTriggerCSharpGet")]
        public static async Task<IActionResult> Get([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req, ILogger log)
        {
            string id = req.Query["id"];

            var result = await Bucket.Value.GetAsync<MyDocument>(id);
            var doc = result.Value;

            return new OkObjectResult(doc);
        }

        [FunctionName("HttpTriggerCSharpSet")]
        public static async Task<IActionResult> Set([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]MyDocument req, ILogger log)
        {
            string id = Guid.NewGuid().ToString();

            await Bucket.Value.InsertAsync(id, req);

            return new OkObjectResult($"New document inserted with ID {id}");
        }        
    }

    public class MyDocument
    {
        public string Name { get; set; }
        public int ShoeSize { get; set; }
        public decimal Balance { get; set; }
    }
}

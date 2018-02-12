using System.Net.Http;
using System.Threading.Tasks;
using AlexaBoothDuty.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace AlexaBoothDuty.Functions
{
    // tag::azurefunction[]
    public static class BoothDuty
    {
        [FunctionName("BoothDuty")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            var speechlet = new BoothDutySpeechlet();
            return await speechlet.GetResponseAsync(req);
        }
    }
    // end::azurefunction[]
}

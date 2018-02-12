using System.Threading.Tasks;
using AlexaSkillsKit.Speechlet;

namespace AlexaBoothDuty.Core.IntentProcessors
{
    public class FallbackIntentProcessor : BaseIntentProcessor
    {
        public override async Task<SpeechletResponse> Execute(IntentRequest intentRequest)
        {
            return await CreatePlainTextSpeechletReponseAsync("Welcome to the Couchbase booth. Ask me about Couchbase.");
        }
    }
}
using System.Threading.Tasks;
using AlexaSkillsKit.Speechlet;

namespace AlexaBoothDuty.Core.IntentProcessors
{
    public class CouchDbIntentProcessor : BaseIntentProcessor
    {
        public override Task<SpeechletResponse> Execute(IntentRequest intentRequest)
        {
            return CreatePlainTextSpeechletReponseAsync("No.");
        }
    }
}
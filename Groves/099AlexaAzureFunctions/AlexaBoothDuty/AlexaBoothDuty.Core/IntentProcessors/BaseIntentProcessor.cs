using System.Threading.Tasks;
using AlexaSkillsKit.Speechlet;
using AlexaSkillsKit.UI;

namespace AlexaBoothDuty.Core.IntentProcessors
{
    public abstract class BaseIntentProcessor : IIntentProcessor
    {
        public abstract Task<SpeechletResponse> Execute(IntentRequest intentRequest);

        protected async Task<SpeechletResponse> CreatePlainTextSpeechletReponseAsync(string text, bool shouldEndSession = false)
        {
            var resp = new SpeechletResponse
            {
                ShouldEndSession = shouldEndSession,
                OutputSpeech = new PlainTextOutputSpeech
                {
                    Text = text
                }
            };
            return await Task.FromResult(resp);
        }

        protected async Task<SpeechletResponse> CreateErrorResponseAsync()
        {
            var resp = new SpeechletResponse
            {
                ShouldEndSession = true,
                OutputSpeech = new PlainTextOutputSpeech
                {
                    Text = "Something went wrong. Please try again later."
                }
            };
            return await Task.FromResult(resp);
        }
    }
}
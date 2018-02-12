using System;
using System.Threading.Tasks;
using AlexaBoothDuty.Core.IntentProcessors;
using AlexaSkillsKit.Authentication;
using AlexaSkillsKit.Json;
using AlexaSkillsKit.Speechlet;
using AlexaSkillsKit.UI;

namespace AlexaBoothDuty.Core
{
    // tag::speechlet[]
    public class BoothDutySpeechlet : SpeechletBase, ISpeechletWithContextAsync
    {
        public async Task<SpeechletResponse> OnIntentAsync(IntentRequest intentRequest, Session session, Context context)
        {
            try
            {
                var intentName = intentRequest.Intent.Name;
                var intentProcessor = IntentProcessor.Create(intentName);
                return await intentProcessor.Execute(intentRequest);
            }
            catch (Exception ex)
            {
                var resp = new SpeechletResponse();
                resp.ShouldEndSession = false;
                resp.OutputSpeech = new PlainTextOutputSpeech() { Text = ex.Message };
                return await Task.FromResult(resp);
            }
        }

        public Task<SpeechletResponse> OnLaunchAsync(LaunchRequest launchRequest, Session session, Context context)
        {
            var resp = new SpeechletResponse();
            resp.ShouldEndSession = false;
            resp.OutputSpeech = new PlainTextOutputSpeech() { Text = "Welcome to the Couchbase booth. Ask me about Couchbase." };
            return Task.FromResult(resp);
        }

        public Task OnSessionStartedAsync(SessionStartedRequest sessionStartedRequest, Session session, Context context)
        {
            return Task.Delay(0); // nothing to do (yet)
        }

        public Task OnSessionEndedAsync(SessionEndedRequest sessionEndedRequest, Session session, Context context)
        {
            return Task.Delay(0); // nothing to do (yet)
        }

        // I only need to use this when I'm testing locally
//        public override bool OnRequestValidation(SpeechletRequestValidationResult result, DateTime referenceTimeUtc,
//            SpeechletRequestEnvelope requestEnvelope)
//        {
//            return true;
//        }
    }
    // end::speechlet[]
}

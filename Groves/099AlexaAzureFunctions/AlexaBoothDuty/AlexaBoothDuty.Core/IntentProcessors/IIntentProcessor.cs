using System.Threading.Tasks;
using AlexaSkillsKit.Speechlet;

namespace AlexaBoothDuty.Core.IntentProcessors
{
    public interface IIntentProcessor
    {
        Task<SpeechletResponse> Execute(IntentRequest intentRequest);
    }
}
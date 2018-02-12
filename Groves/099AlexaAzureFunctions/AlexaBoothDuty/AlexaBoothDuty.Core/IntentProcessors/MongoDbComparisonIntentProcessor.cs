using System.Linq;
using System.Threading.Tasks;
using AlexaSkillsKit.Speechlet;
using Couchbase.Core;
using Couchbase.N1QL;

namespace AlexaBoothDuty.Core.IntentProcessors
{
    public class MongoDbComparisonIntentProcessor : BaseIntentProcessor
    {
        private readonly IBucket _bucket;

        public MongoDbComparisonIntentProcessor(IBucket bucket)
        {
            _bucket = bucket;
        }

        public override async Task<SpeechletResponse> Execute(IntentRequest intentRequest)
        {
            // get random fact from bucket
            var n1ql = @"select m.*, meta(m).id
                            from boothduty m
                            where m.type = 'mongodbcomparison'
                            order by `number`, uuid()
                            limit 1;";
            var query = QueryRequest.Create(n1ql);
            var result = await _bucket.QueryAsync<BoothFact>(query);
            if (result == null || !result.Rows.Any())
                return await CreateErrorResponseAsync();
            var fact = result.First();

            // increment fact count
            await _bucket.MutateIn<dynamic>(fact.Id)
                .Counter("number", 1)
                .ExecuteAsync();

            // return text of fact
            return await CreatePlainTextSpeechletReponseAsync(fact.Text);
        }
    }
}
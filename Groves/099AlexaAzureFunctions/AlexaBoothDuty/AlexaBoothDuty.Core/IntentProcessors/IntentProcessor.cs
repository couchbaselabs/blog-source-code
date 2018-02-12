namespace AlexaBoothDuty.Core.IntentProcessors
{
    public static class IntentProcessor
    {
        // tag::Create[]
        public static IIntentProcessor Create(string intentName = "FallbackIntent")
        {
            switch (intentName)
            {
                case "MongodbComparisonIntent":
                    return new MongoDbComparisonIntentProcessor(CouchbaseBucket.GetBucket());
                case "WhatIsCouchbaseIntent":
                    return new WhatIsCouchbaseIntentProcessor(CouchbaseBucket.GetBucket());
                case "CouchDbIntent":
                    return new CouchDbIntentProcessor();
                case "FallbackIntent":
                    return new FallbackIntentProcessor();
                default:
                    return new FallbackIntentProcessor();
            }
        }
        // end::Create[]
    }
}
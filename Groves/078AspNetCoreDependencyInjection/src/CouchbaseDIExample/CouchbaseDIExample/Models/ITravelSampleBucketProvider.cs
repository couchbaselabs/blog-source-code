using Couchbase.Extensions.DependencyInjection;

namespace CouchbaseDIExample.Models
{
    // tag::ITravelSampleBucketProvider[]
    public interface ITravelSampleBucketProvider : INamedBucketProvider
    {
        // nothing goes in here!
    }
    // end::ITravelSampleBucketProvider[]
}
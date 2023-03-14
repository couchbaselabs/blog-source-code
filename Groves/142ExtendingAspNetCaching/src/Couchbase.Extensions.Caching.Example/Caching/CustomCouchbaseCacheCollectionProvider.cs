using Couchbase.KeyValue;

namespace Couchbase.Extensions.Caching.Example.Caching;

public class CustomCouchbaseCacheCollectionProvider : ICouchbaseCacheCollectionProvider
{
    private readonly ICouchbaseCacheBucketProvider _bucketProvider;
    private readonly string _scopeName;
    private readonly string _collectionName;

    public CustomCouchbaseCacheCollectionProvider(ICouchbaseCacheBucketProvider bucketProvider, string scopeName, string collectionName)
    {
        _bucketProvider = bucketProvider;
        _scopeName = scopeName;
        _collectionName = collectionName;
    }

    public async ValueTask<ICouchbaseCollection> GetCollectionAsync()
    {
        var bucket = await _bucketProvider.GetBucketAsync().ConfigureAwait(false);

        var scope = await bucket.ScopeAsync(_scopeName);

        return await scope.CollectionAsync(_collectionName);
    }
}
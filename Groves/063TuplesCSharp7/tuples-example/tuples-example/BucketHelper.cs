using Couchbase;
using Couchbase.Core;

namespace tuplesExample
{
    // tag::BucketHelper[]
    public class BucketHelper
    {
        private readonly IBucket _bucket;

        public BucketHelper(IBucket bucket)
        {
            _bucket = bucket;
        }

        public (string Key, T obj) GetTuple<T>(string key)
        {
            var doc = _bucket.Get<T>(key);
            return (doc.Id, doc.Value);
        }

        public void InsertTuple<T>((string Key, T obj) tuple)
        {
            _bucket.Insert(new Document<T>
            {
                Id = tuple.Key,
                Content = tuple.obj
            });
        }
    }
    // end::BucketHelper[]
}

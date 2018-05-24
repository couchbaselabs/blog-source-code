using System;
using Couchbase;
using Couchbase.Core;

namespace AcidPart2
{
//    public static class CouchbaseHelpers
//    {
//        public static void UpdateWithCas<T>(this IBucket @this, string documentId, Action<T> act, ulong? cas = null)
//        {
//            var document = @this.Get<T>(documentId);
//            var content = document.Value;
//            act(content);
//            @this.Replace(new Document<T>
//            {
//                Cas = cas ?? document.Cas,
//                Id = document.Id,
//                Content = content
//            });
//            // NOTE: could put retr(ies) here
//        }
//    }
}
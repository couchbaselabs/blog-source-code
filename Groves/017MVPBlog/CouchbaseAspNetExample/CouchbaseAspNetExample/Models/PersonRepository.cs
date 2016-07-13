using System;
using System.Collections.Generic;
using System.Linq;
using Couchbase.Core;
using Couchbase.Linq;
using Couchbase.Linq.Extensions;
using Couchbase.N1QL;

namespace CouchbaseAspNetExample3.Models
{
    public class PersonRepository
    {
        private readonly IBucket _bucket;
        private readonly IBucketContext _context;

        public PersonRepository(IBucket bucket, IBucketContext context)
        {
            _bucket = bucket;
            _context = context;
        }

        public List<Person> GetAll()
        {
            return _context.Query<Person>()
               .ScanConsistency(ScanConsistency.RequestPlus)   // waiting for the indexing to complete before it returns a response
               .OrderBy(p => p.Name)
               .ToList();
        }

        public Person GetPersonByKey(Guid key)
        {
            var person = _bucket.Get<Person>("Person::" + key).Value;
            return person;
        }

        public void Save(Person person)
        {
            // if there is no ID, then assume this is a "new" person
            // and assign an ID
            if (string.IsNullOrEmpty(person.Id))
                person.Id = "Person::" + Guid.NewGuid();

            _context.Save(person);

// alternate: with plain .NET SDK
//            var doc = new Document<Person>
//            {
//                Id = "Person::" + person.Id,
//                Content = person
//            };
//            _bucket.Upsert(doc);
        }

        public void Delete(Guid id)
        {
            // you could use _context.Remove(document); if you have the whole document
            _bucket.Remove("Person::" + id);
        }
    }
}
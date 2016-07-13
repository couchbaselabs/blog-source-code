using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Couchbase.Linq.Filters;

namespace CouchbaseAspNetExample3.Models
{
    [DocumentTypeFilter("Person")]
    public class Person
    {
        public Person() { Type = "Person"; }

        [Key]
        public string Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; } 
        public string Address { get; set; }
        
        // iterations
        public string PhoneNumber { get; set; }
        public List<string> FavoriteMovies { get; set; }
    }
}
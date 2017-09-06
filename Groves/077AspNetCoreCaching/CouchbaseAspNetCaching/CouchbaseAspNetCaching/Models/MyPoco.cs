using System;

namespace CouchbaseAspNetCaching.Models
{
    // tag::mypoco[]
    public class MyPoco
    {
        public string Name { get; set; }
        public int ShoeSize { get; set; }
        public decimal Price { get; set; }
        // end::mypoco[]

        public static MyPoco Generate()
        {
            // I'm using the Faker library to generate realistic looking info
            // It is not required at all to do this
            return new MyPoco
            {
                Name = Faker.Name.FullName(),
                ShoeSize = Faker.RandomNumber.Next(5,15),
                Price = Convert.ToDecimal(Faker.RandomNumber.Next(1,100)) - 0.01M
            };
        }
    }
}

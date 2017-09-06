using System;

namespace CouchbaseAspNetCaching.Models
{
    public class MyPoco
    {
        public string Name { get; set; }
        public int ShoeSize { get; set; }
        public decimal Price { get; set; }

        public static MyPoco Generate()
        {
            return new MyPoco
            {
                Name = Faker.Name.FullName(),
                ShoeSize = Faker.RandomNumber.Next(5,15),
                Price = Convert.ToDecimal(Faker.RandomNumber.Next(1,100)) - 0.01M
            };
        }
    }
}

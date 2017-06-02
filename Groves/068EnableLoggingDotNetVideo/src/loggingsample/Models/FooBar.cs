using System;
using System.IO;

namespace loggingsample.Models
{
    public class FooBar
    {
        public string Foo { get; set; }
        public string Bar { get; set; }

        public static FooBar Generate()
        {
            return new FooBar
            {
                Foo = Path.GetRandomFileName(),
                Bar = new Random().Next(1, 1000000).ToString()
            };
        }
    }
}
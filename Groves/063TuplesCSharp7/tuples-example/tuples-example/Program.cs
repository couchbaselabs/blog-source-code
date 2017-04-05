using Couchbase;
using Couchbase.Configuration.Client;
using Couchbase.Core;
using System;
using System.Collections.Generic;
using System.IO;

namespace tuplesExample
{
    class Program
    {
        private const string bucketName = "default";
        private const string connectionString = "couchbase://localhost";
        private const string username = "Administrator";
        private const string password = "password";

        static void Main(string[] args)
        {
            // initialize couchbase, make sure it has index and data
            ClusterHelper.Initialize(new ClientConfiguration
            {
                Servers = new List<Uri> {  new Uri(connectionString) }
            });
            var bucket = ClusterHelper.GetBucket(bucketName);
            SetupFilmData(bucket);

            // tag::GetTuple[]
            var bucketHelper = new BucketHelper(bucket);

            (string key, Film film) fightClub = bucketHelper.GetTuple<Film>("film-001");
            // end::GetTuple[]

            WriteFilmToConsole(fightClub);

            // create a new random film
            // tag::InsertTuple[]
            string key = Guid.NewGuid().ToString();
            Film randomFilm = GenerateRandomFilm();
            bucketHelper.InsertTuple((key, randomFilm));
            // end::InsertTuple[]

            // get the random film back out and display it
            var randomFilmBackOut = bucketHelper.GetTuple<Film>(key);
            WriteFilmToConsole(randomFilmBackOut);

            // clean up
            ClusterHelper.Close();
        }

        private static void WriteFilmToConsole((string Id, Film Film) film)
        {
            Console.WriteLine($"Movie Id: {film.Id}");
            Console.WriteLine($"Movie Information:");
            Console.WriteLine($"\tTitle: {film.Film.Title}");
            Console.WriteLine($"\tYear: {film.Film.Year}");
            Console.WriteLine($"\tGenre: {film.Film.Genre}");
            Console.WriteLine();
        }

        private static Film GenerateRandomFilm()
        {
            return new Film
            {
                Title = "The Adventures of " + Path.GetRandomFileName(),
                Year = new Random().Next(1920, DateTime.Now.Year),
                Genre = Path.GetRandomFileName()
            };
        }

        // this method just ensures there is some data in the bucket
        private static void SetupFilmData(IBucket bucket)
        {
            // make sure there's a primary index
            var manager = bucket.CreateManager(username, password);
            manager.CreateN1qlPrimaryIndex(false);

            if(!bucket.Exists("film-001"))
                bucket.Insert(new Document<Film>
                {
                    Id = "film-001",
                    Content = new Film { Title = "Fight Club", Year = 1999, Genre = "drama" }
                });
            if (!bucket.Exists("film-002"))
                bucket.Insert(new Document<Film>
                {
                    Id = "film-002",
                    Content = new Film { Title = "Casablanca", Year = 1942, Genre = "war" }
                });
            if (!bucket.Exists("film-003"))
                bucket.Insert(new Document<Film>
                {
                    Id = "film-003",
                    Content = new Film { Title = "Citizen Kane", Year = 1942, Genre = "mystery" }
                });
        }
    }
}
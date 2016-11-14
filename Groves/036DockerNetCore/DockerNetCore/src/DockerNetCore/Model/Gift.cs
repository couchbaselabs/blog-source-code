using System;
using System.Collections.Generic;
using Couchbase;
using Couchbase.N1QL;

namespace DockerNetCore.Model
{
    public class Gift
    {
        public string GiftName { get; set; }
        public decimal Price { get; set; }

        // tag::GetAllGifts[]
        public static List<Gift> GetAllGifts()
        {
            var bucket = ClusterHelper.GetBucket("default");
            var query = QueryRequest.Create("SELECT g.* FROM `default` g");
            query.ScanConsistency(ScanConsistency.RequestPlus);
            return bucket.Query<Gift>(query).Rows;
        }
        // end::GetAllGifts[]

        public static void CreateNewGift()
        {
            var gift = new Gift();
            gift.GiftName = GetRandomGiftName();
            gift.Price = new Random().Next(1000, 5000)/100.0M;
            var bucket = ClusterHelper.GetBucket("default");
            bucket.Insert(new Document<Gift> {Id = Guid.NewGuid().ToString(), Content = gift});
        }

        private static string GetRandomGiftName()
        {
            var rand = new Random();
            var giftNames = new List<string>
            {
                "Candy", "Nuts", "Rocking horse", "Doll", "Mittens", "Gloves", "Toy train", "Books", "Handkerchief", "Skates",
                "Furby", "Robopuppy", "iPad", "Lego", "Nerf", "BB-8", "Shopkins"
            };
            return giftNames[rand.Next(0, giftNames.Count - 1)] + " #" + rand.Next(100,999);
        }
    }
}
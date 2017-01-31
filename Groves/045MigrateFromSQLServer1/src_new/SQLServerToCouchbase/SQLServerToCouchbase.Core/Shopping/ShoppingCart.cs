using System;
using System.Collections.Generic;

namespace SQLServerToCouchbase.Core.Shopping
{
    public class ShoppingCart
    {
        public Guid Id { get; set; }
        public string User { get; set; }
        public DateTime DateCreated { get; set; }

        public List<Item> Items { get; set; }
    }
}
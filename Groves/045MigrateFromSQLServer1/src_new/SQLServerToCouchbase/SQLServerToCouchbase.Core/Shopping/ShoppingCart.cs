using System;
using System.Collections.Generic;
using System.Linq;

namespace SQLServerToCouchbase.Core.Shopping
{
    public class ShoppingCart
    {
        public Guid Id { get; set; }
        public string User { get; set; }
        public DateTime DateCreated { get; set; }

        public List<Item> Items { get; set; }

        public int ItemCount
        {
            get
            {
                if (Items == null || !Items.Any())
                    return 0;
                return Items.Sum(i => i.Quantity);
            }
        }

        public decimal ItemTotal
        {
            get
            {
                if (Items == null || !Items.Any())
                    return 0;
                return Items.Sum(i => i.Total);
            }
        }
    }
}
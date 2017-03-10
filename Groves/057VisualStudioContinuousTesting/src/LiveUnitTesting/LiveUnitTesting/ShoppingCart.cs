using System;
using System.Collections.Generic;
using System.Linq;

namespace ContinuousTesting
{
    // tag::ShoppingCart[]
    public class ShoppingCart
    {
        public string UserName { get; set; }
        public DateTime LastUpdated { get; set; }
        public List<Item> Items { get; set; }
        public decimal Total
        {
            get { return Items.Sum(i => i.Price); }
        }
    }
    // end::ShoppingCart[]
}

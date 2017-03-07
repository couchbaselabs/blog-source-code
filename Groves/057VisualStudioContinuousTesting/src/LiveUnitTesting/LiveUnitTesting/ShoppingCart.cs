using System;
using System.Collections.Generic;
using System.Text;

namespace ContinuousTesting
{
    public class ShoppingCart
    {
        public string UserName { get; set; }
        public DateTime LastUpdated { get; set; }
        public List<Item> Items { get; set; }
    }
}

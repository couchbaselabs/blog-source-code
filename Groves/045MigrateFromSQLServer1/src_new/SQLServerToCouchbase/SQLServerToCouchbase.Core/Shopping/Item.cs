using System;

namespace SQLServerToCouchbase.Core.Shopping
{
    public class Item
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Total => Price * Quantity;

        public Guid ShoppingCartId { get; set; }
    }
}
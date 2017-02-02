using System;

namespace SQLServerToCouchbase.Core.Shopping
{
    // tag::Item[]
    public class Item
    {
        // end::Item[]
        // tag::Item2[]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        // end::Item2[]
        public decimal Total => Price * Quantity;

        public Guid ShoppingCartId { get; set; }
        // tag::Item3[]
    }
    // end::Item3[]
}
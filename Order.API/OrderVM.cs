using System.Collections.Generic;

namespace Order.API
{
    public class OrderVM
    {
        public int BuyerId { get; set; }
        public List<OrderItemVM> OrderItems { get; set; }
    }

    public class OrderItemVM
    {
        public int ProductId { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
    }
}

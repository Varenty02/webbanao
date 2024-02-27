using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using webbanao.Models.Product;

namespace webbanao.Models.Order
{
    public class OrderItem
    {
        public int ProductId { get; set; }
        public int OrderId {  get; set; }
        public int Quantity { get; set; }
        [ForeignKey(nameof(OrderId))]
        public OrderModel Order {  get; set; }
        [ForeignKey(nameof(ProductId))]
        public ProductModel Product { get; set; }

    }
}

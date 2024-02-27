using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webbanao.Models.Order
{
    public class OrderModel
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public int Total { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        [ForeignKey(nameof(UserId))]
        public AppUser Customer { get; set; }
        public ICollection<OrderItem> Items { get; set; }
    }
}

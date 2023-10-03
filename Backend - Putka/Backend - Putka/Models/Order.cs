using Backend___Putka.Enums;
using System.ComponentModel.DataAnnotations;

namespace Backend___Putka.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string AppUserId { get; set; }
        [MaxLength(50)]
        public string UserName { get; set; }
        [Required]
        [MaxLength(250)]
        public string Email { get; set; }
        [MaxLength(50)]
        public string Phone { get; set; }
        [MaxLength(200)]
        public string Address { get; set; }
        [MaxLength(250)]
        public string Note { get; set; }
        public DateTime CreatedDate { get; set; }
        public OrderStatus Status { get; set; }
        public AppUser AppUser { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}

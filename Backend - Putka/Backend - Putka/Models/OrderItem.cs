namespace Backend___Putka.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Count { get; set; }
        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}

using Backend___Putka.Models;

namespace Backend___Putka.ViewModels
{
    public class CheckoutItem
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
        public List<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    }
}

using Backend___Putka.Models;

namespace Backend___Putka.ViewModels
{
    public class ShopViewModel
    {
        public List<Product> Products { get; set; }
        public List<Category> Categories { get; set; }
        public List<Tag> Tags { get; set; }
        public List<Weight> Weights { get; set; }

    }
}

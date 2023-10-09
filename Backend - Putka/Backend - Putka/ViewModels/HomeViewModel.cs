using Backend___Putka.Models;

namespace Backend___Putka.ViewModels
{
    public class HomeViewModel
    {
        public List<Product> Products { get; set; }
        public List<Product> BestSellerProducts { get; set; }
        public List<Product> TrendingProducts { get; set; }

        public List<Category> Categories { get; set; }
    }
}

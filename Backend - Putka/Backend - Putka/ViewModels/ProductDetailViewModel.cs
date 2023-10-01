using Backend___Putka.Models;

namespace Backend___Putka.ViewModels
{
    public class ProductDetailViewModel
    {
        public Product Product { get; set; }
        public List<Product> RelatedProducts { get; set; }
        //public List<Feature> Features { get; set; } = new List<Feature>();
        public ProductComment ProductComment { get; set; }
    }
}

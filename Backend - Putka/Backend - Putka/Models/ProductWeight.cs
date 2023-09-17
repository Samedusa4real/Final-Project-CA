namespace Backend___Putka.Models
{
    public class ProductWeight
    {
        public int ProductId { get; set; }
        public int WeightId { get; set; }
        public Product Product { get; set; }
        public Weight Weight { get; set; }
    }
}

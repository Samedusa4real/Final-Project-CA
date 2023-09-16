using System.ComponentModel.DataAnnotations;

namespace Backend___Putka.Models
{
    public class ProductImage
    {
        public int Id { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        [MaxLength(100)]
        public string Url { get; set; }
        [Required]
        public bool IsMain { get; set; }
        public Product Product { get; set; }
    }
}

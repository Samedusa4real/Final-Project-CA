using System.ComponentModel.DataAnnotations;

namespace Backend___Putka.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(25)]
        public string Name { get; set; }
        [Required]
        [MaxLength(250)]
        public string Icon { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();

    }
}

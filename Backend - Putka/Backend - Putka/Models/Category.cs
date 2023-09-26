using Backend___Putka.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [MaxFileSize(10)]
        [AllowedExtensions("image/jpeg", "image/png")]
        [NotMapped]
        public IFormFile ImageFile { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Backend___Putka.Models
{
    public class ProductComment
    {
        public int Id { get; set; }
        public string AppUserId { get; set; }
        public int ProductId { get; set; }
        [Required]
        [MaxLength(500)]
        public string Comment { get; set; }
        [Required]
        [Range(1, 5)]
        public int Rate { get; set; }
        public DateTime CreatedDate { get; set; }
        public AppUser AppUser { get; set; }
        public Product Product { get; set; }
    }
}

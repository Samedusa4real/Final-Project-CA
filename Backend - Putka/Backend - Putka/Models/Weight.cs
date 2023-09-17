using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend___Putka.Models
{
    public class Weight
    {
        public int Id { get; set; }
        [Required]
        [Column(TypeName = "money")]
        public decimal Name { get; set; }
    }
}

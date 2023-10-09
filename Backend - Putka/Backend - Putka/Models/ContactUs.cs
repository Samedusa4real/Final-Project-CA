using System.ComponentModel.DataAnnotations;

namespace Backend___Putka.Models
{
    public class ContactUs
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(25)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(25)]
        public string LastName { get; set; }
        [Required]
        [MaxLength(250)]
        public string Email { get; set; }
        [MaxLength(250)]
        public string Phone { get; set; }
        [MaxLength(500)]
        public string Message { get; set; }
    }
}

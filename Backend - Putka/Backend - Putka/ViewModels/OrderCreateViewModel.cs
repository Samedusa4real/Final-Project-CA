using System.ComponentModel.DataAnnotations;

namespace Backend___Putka.ViewModels
{
    public class OrderCreateViewModel
    {
        [MaxLength(50)]
        public string UserName { get; set; }
        [MaxLength(250)]
        public string Email { get; set; }
        [MaxLength(50)]
        public string Phone { get; set; }
        [MaxLength(200)]
        public string Address { get; set; }
        [MaxLength(250)]
        public string Note { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Backend___Putka.ViewModels
{
    public class MemberLoginViewModel
    {
        [Required]
        [MaxLength(20)]
        public string UserName { get; set; }
        [Required]
        [MaxLength(20)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}

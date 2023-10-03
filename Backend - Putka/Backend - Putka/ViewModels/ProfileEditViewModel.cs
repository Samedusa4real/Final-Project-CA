using System.ComponentModel.DataAnnotations;

namespace Backend___Putka.ViewModels
{
    public class ProfileEditViewModel
    {
        [Required]
        [MaxLength(50)]
        public string UserName { get; set; }
        [Required]
        [MaxLength(250)]
        public string Email { get; set; }
        [MaxLength(50)]
        public string Phone { get; set; }
        [MaxLength(200)]
        public string Address { get; set; }
        [MaxLength(20)]
        public string CurrentPassword { get; set; }
        [MaxLength(20)]
        public string NewPassword { get; set; }
        [Compare(nameof(NewPassword))]
        public string ConfirmNewPassword { get; set; }
    }
}

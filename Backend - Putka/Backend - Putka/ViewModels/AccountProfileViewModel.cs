using Backend___Putka.Models;

namespace Backend___Putka.ViewModels
{
    public class AccountProfileViewModel
    {
        public ProfileEditViewModel Profile { get; set; }
        public List<Order> Orders { get; set; }
    }
}

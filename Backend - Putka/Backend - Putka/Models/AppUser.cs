using Microsoft.AspNetCore.Identity;

namespace Backend___Putka.Models
{
    public class AppUser:IdentityUser
    {
        public string FullName { get; set; }
        public bool IsAdmin { get; set; }
        public string Address { get; set; }
    }
}

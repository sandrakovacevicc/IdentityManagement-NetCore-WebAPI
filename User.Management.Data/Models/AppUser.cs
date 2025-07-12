using Microsoft.AspNetCore.Identity;

namespace UserManagement.Data.Models
{
    public class AppUser : IdentityUser
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiry { get; set; }
    }
}

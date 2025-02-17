using Microsoft.AspNetCore.Identity;


namespace EventTracker.Models
{
    public class User : IdentityUser
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
    }
}

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EventTracker.Models
{
    public class User : IdentityUser
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
    }
}

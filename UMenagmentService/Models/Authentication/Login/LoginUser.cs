using System.ComponentModel.DataAnnotations;

namespace UserManagment.Service.Models.Authentication.Login
{
    public class LoginUser
    {
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}

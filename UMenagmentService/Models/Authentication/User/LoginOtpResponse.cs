using UserManagement.Data.Models;

namespace UserManagment.Service.Models.Authentication.User
{
    public class LoginOtpResponse
    {
        public string Token { get; set; } = null;

        public bool IsTwoFactorEnable { get; set; }

        public AppUser User { get; set; } = null;
    }
}

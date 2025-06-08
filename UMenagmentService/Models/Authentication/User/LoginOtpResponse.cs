
namespace UMenagmentService.Models
{
    public class LoginOtpResponse
    {
        public string Token { get; set; } = null;

        public bool IsTwoFactorEnable { get; set; }

        public User User { get; set; } = null;
    }
}

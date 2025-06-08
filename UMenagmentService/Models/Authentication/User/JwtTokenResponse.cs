
namespace UMenagmentService.Models.Authentication
{
    public class JwtTokenResponse
    {
        public string Token { get; set; } = null;
        public DateTime Expiration { get; set; }
    }
}

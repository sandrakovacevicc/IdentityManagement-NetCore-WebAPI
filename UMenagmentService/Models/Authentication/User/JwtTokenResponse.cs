namespace UserManagment.Service.Models.Authentication.User
{
    public class JwtTokenResponse
    {
        public string Token { get; set; } = null;
        public DateTime Expiration { get; set; }
    }
}

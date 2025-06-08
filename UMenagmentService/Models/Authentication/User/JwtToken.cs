namespace UserManagment.Service.Models.Authentication.User
{
    public class JwtToken
    {
        public string Token { get; set; } = null;
        public DateTime ExpiryTokenDate { get; set; }
    }
}

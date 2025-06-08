using UserManagement.Data.Models;

namespace UserManagment.Service.Models.Authentication.User
{
    public class CreateUserResponse
    {
        public string Token { get; set; }

        public AppUser User { get; set; }
    }
}

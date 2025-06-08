using UserManagement.Data.Models;
using UserManagment.Service.Models;
using UserManagment.Service.Models.Authentication.Login;
using UserManagment.Service.Models.Authentication.Register;
using UserManagment.Service.Models.Authentication.User;

namespace UserManagment.Service.Service
{
    public interface IUManagement
    {
        Task<ApiResponse<CreateUserResponse>> CreateUserWithTokenAsync(RegisterUser registerRequest);

        Task<ApiResponse<LoginOtpResponse>> GetOtpByLoginAsync(LoginUser loginUser);

        Task<ApiResponse<JwtTokenResponse>> GenerateJwtTokenAsync(AppUser user);
    }
}

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

        Task<ApiResponse<LoginResponse>> LoginUserWithJWTTokenAsync(string otp, string userName);

        Task<ApiResponse<LoginOtpResponse>> GetOtpByLoginAsync(LoginUser user);

        Task<ApiResponse<LoginResponse>> GetJwtTokenAsync(AppUser user);

        Task<ApiResponse<LoginResponse>> RenewAccessTokenAsync(LoginResponse tokens);
    }
}

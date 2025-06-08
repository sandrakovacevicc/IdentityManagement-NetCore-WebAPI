using UMenagmentService.Models;
using UMenagmentService.Models.Authentication;
using UMenagmentService.Models.Authentication.Login;
using UMenagmentService.Models.Authentication.Register;

namespace UMenagmentService.Service
{
    public interface IUserManagement
    {
        Task<ApiResponse<CreateUserResponse>> CreateUserWithTokenAsync(RegisterUser registerRequest);

        Task<ApiResponse<LoginOtpResponse>> GetOtpByLoginAsync(LoginUser loginUser);

        Task<ApiResponse<JwtTokenResponse>> GenerateJwtTokenAsync(User user);
    }
}

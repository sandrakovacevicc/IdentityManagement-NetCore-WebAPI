using UMenagmentService.Models;
using UMenagmentService.Models.Authentication.Register;
using UMenagmentService.Models.Authentication.UserResponses;

namespace UMenagmentService.Service
{
    public interface IUserManagement
    {
        Task<ApiResponse<CreateUserResponse>> CreateUserWithTokenAsync(RegisterUser registerRequest);
    }
}

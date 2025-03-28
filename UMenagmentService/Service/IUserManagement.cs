using UMenagmentService.Models;
using UMenagmentService.Models.Authentication.Register;

namespace UMenagmentService.Service
{
    public interface IUserManagement
    {
        Task<ApiResponse<string>> CreateUserWithTokenAsync(RegisterUser registerRequest);
    }
}

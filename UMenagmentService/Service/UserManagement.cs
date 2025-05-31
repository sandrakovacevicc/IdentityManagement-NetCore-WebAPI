using Microsoft.AspNetCore.Identity;
using UMenagmentService.Models;
using UMenagmentService.Models.Authentication.Register;
using Microsoft.Extensions.Configuration;
using UMenagmentService.Models.Authentication.UserResponses;

namespace UMenagmentService.Service
{
    public class UserManagement : IUserManagement
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public UserManagement(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager, IConfiguration configuration, RoleManager<IdentityRole> roleManager, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        public async Task<ApiResponse<CreateUserResponse>> CreateUserWithTokenAsync(RegisterUser registerRequest)
        {

            var userExist = await _userManager.FindByEmailAsync(registerRequest.Email);
            if (userExist != null)
            {
                return new ApiResponse<CreateUserResponse> { IsSuccess = false, StatusCode = 403, Message = "User already exists!" };
            }

            User user = new()
            {
                Email = registerRequest.Email,
                Name = registerRequest.Name,
                Surname = registerRequest.Surname,
                UserName = registerRequest.Email,
                PhoneNumber = registerRequest.PhoneNumber,
                TwoFactorEnabled = true,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            if (!await _roleManager.RoleExistsAsync("Client"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Client"));
            }

     
            var result = await _userManager.CreateAsync(user, registerRequest.Password);

            if (!result.Succeeded)
            {
                return new ApiResponse<CreateUserResponse> { IsSuccess = false, StatusCode = 500, Message = "User failed to register." };
            }

            await _userManager.AddToRoleAsync(user, "Client");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            return new ApiResponse<CreateUserResponse> { Response = new CreateUserResponse() { User = user, Token = token }, IsSuccess = true, StatusCode = 201, Message = $"User registered & Email Sent to {user.Email} successfully! Please login!" };

        }
    }
}
